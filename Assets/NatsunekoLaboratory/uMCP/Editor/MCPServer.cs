using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using NatsunekoLaboratory.uMCP.Extensions;
using NatsunekoLaboratory.uMCP.Protocol.Attributes;
using NatsunekoLaboratory.uMCP.Protocol.Errors;
using NatsunekoLaboratory.uMCP.Protocol.Interfaces;
using NatsunekoLaboratory.uMCP.Protocol.Json;
using NatsunekoLaboratory.uMCP.Protocol.Response;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEditor;

using UnityEngine;

namespace NatsunekoLaboratory.uMCP
{
    /// <summary>
    ///     this class implements the streamable MCP server for controlling Unity that focuses on animation creating and
    ///     editing.
    /// </summary>
    [InitializeOnLoad]
    public class UnityAnimationMcp
    {
        private static HttpListener _listener;
        private static bool _isRunning;

        static UnityAnimationMcp()
        {
            StartServer();
            EditorApplication.quitting += StopServer;
        }

        private static void StartServer()
        {
            StopServer();

            if (_isRunning) return;

            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:7225/");
            _listener.Start();
            _isRunning = true;

            Debug.Log("uMCP Server Started at http://localhost:7225/sse");
            Task.Run(ListenerLoop);
        }

        private static void StopServer()
        {
            if (_listener != null)
            {
                _listener.Stop();
                _listener = null;
                _isRunning = false;
            }
        }

        private static async Task ListenerLoop()
        {
            while (_isRunning)
            {
                var context = await _listener.GetContextAsync();
                var request = context.Request;

                switch (request.Url.AbsolutePath)
                {
                    case "/sse":
                        await HandleRootRequest(context);
                        break;

                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        context.Response.Close();
                        break;
                }
            }
        }

        private static async Task HandleRootRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            try
            {
                switch (request.HttpMethod)
                {
                    case "GET":
                    case "PUT":
                    case "DELETE":
                    case "PATCH":
                    case "POST":
                        var body = GetRequest<JObject>(request);
                        response.ContentType = "text/event-stream";
                        response.Headers.Add("Cache-Control", "no-cache");
                        response.KeepAlive = true;

                        if (body != null)
                            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(body));

                        var stream = response.OutputStream;

                        try
                        {
                            var obj = body?.Method switch
                            {
                                "initialize" => await HandleInitializeRequest(body),
                                "notifications/initialized" => null,
                                "tools/list" => await HandleToolListRequest(body),
                                "tools/call" => await HandleToolCallRequest(body),
                                null => null,
                                _ => null
                            };

                            var json = JsonConvert.SerializeObject(obj);
                            var data = Encoding.UTF8.GetBytes($"data: {json}\n\n");
                            await stream.WriteAsync(data, 0, data.Length);
                            await stream.FlushAsync();
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"MCP Server Error: {e.Message}");

                            var data = Encoding.UTF8.GetBytes("data: null");
                            await response.OutputStream.WriteAsync(data, 0, data.Length);
                            await response.OutputStream.FlushAsync();
                        }


                        break;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            finally
            {
                response.Close();
            }
        }

        private static async Task<JsonRpcResponse> HandleInitializeRequest(JsonRpcRequest<JObject> request)
        {
            var result = await ServerCapabilities.CreateAsync();
            return new JsonRpcSuccessResponse<ServerCapabilities> { JsonRpc = "2.0", Id = request.Id, Result = result };
        }

        private static async Task<JsonRpcResponse> HandleToolListRequest(JsonRpcRequest<JObject> request)
        {
            var result = await ToolList.CreateAsync();
            return new JsonRpcSuccessResponse<ToolList> { JsonRpc = "2.0", Id = request.Id, Result = result };
        }

        private static async Task<JsonRpcResponse> HandleToolCallRequest(JsonRpcRequest<JObject> request)
        {
            var types = Assembly.GetAssembly(typeof(ToolList)).DefinedTypes.Where(w => w.GetCustomAttribute<McpServerToolTypeAttribute>() != null).ToList();
            var tools = types.SelectMany(w => w.GetMethods().Where(v => v.GetCustomAttribute<McpServerToolAttribute>() != null));
            var tool = tools.FirstOrDefault(w => w.Name == request.Params["name"]?.ToString());
            if (tool == null)
                return new JsonRpcErrorResponse<ErrorAboutTool>
                {
                    JsonRpc = "2.0",
                    Id = request.Id,
                    Error = new ErrorResponse<ErrorAboutTool>
                    {
                        Code = -32000,
                        Message = "Tool not found",
                        Data = new ErrorAboutTool { ToolName = request.Params["name"]?.ToString() }
                    }
                };

            var arguments = request.Params["arguments"];
            var parameters = new List<object>();
            foreach (var parameter in tool.GetParameters())
            {
                var argument = arguments?[parameter.Name];
                if (argument == null && parameter.HasCustomAttribute<RequiredAttribute>())
                    return new JsonRpcErrorResponse<ErrorAboutTool>
                    {
                        JsonRpc = "2.0",
                        Id = request.Id,
                        Error = new ErrorResponse<ErrorAboutTool>
                        {
                            Code = -32000,
                            Message = $"Required argument '{parameter.Name}' is missing.",
                            Data = new ErrorAboutTool { ToolName = tool.Name }
                        }
                    };

                var value = parameter.ParameterType switch
                {
                    _ when parameter.ParameterType == typeof(string) => argument?.ToString(),
                    _ when parameter.ParameterType == typeof(int) => argument?.ToObject<int>(),
                    _ when parameter.ParameterType == typeof(long) => argument?.ToObject<long>(),
                    _ when parameter.ParameterType == typeof(float) => argument?.ToObject<float>(),
                    _ when parameter.ParameterType == typeof(double) => argument?.ToObject<double>(),
                    _ when parameter.ParameterType == typeof(bool) => argument?.ToObject<bool>(),
                    _ when parameter.ParameterType.IsEnum => Enum.Parse(parameter.ParameterType, argument?.ToString()),
                    _ when parameter.ParameterType.IsArray => argument?.ToObject(parameter.ParameterType),
                    _ => throw new ArgumentOutOfRangeException()
                };

                parameters.Add(value);
            }

            var contents = ToContent(tool.Invoke(null, parameters.ToArray()));
            return new JsonRpcSuccessResponse<CallToolResults> { JsonRpc = "2.0", Id = request.Id, Result = new CallToolResults { Content = contents.ToArray() } };
        }

        private static List<CallToolResultContent> ToContent(object content)
        {
            return content switch
            {
                ITextResult text => new List<CallToolResultContent> { new CallToolTextContent { Text = text.Text } },
                IImageResult img => new List<CallToolResultContent> { new CallToolImageContent { Data = img.Image.ToBase64String(ImageFormat.Png), MimeType = "image/png" } },
                IReferenceResult r => new List<CallToolResultContent> { new CallToolReferenceContent { Resource = new Resource { Uri = r.Reference, Name = r.FileName, Description = r.Description } } },
                not null when content.GetType().IsArray => ((IEnumerable<object>)content).Select(ToContent).SelectMany(x => x).ToList(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static JsonRpcRequest<T> GetRequest<T>(HttpListenerRequest request)
        {
            using var reader = new StreamReader(request.InputStream, Encoding.UTF8);
            var json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<JsonRpcRequest<T>>(json);
        }
    }
}
