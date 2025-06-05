using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<long, (JsonRpcRequest<JObject> Object, TaskCompletionSource<JsonRpcResponse> TaskCompletionSource)> _queue = new();
        private static readonly object LockObj = new();
        private static readonly List<MethodInfo> Tools;

        private static readonly JsonSerializerSettings SerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        static UnityAnimationMcp()
        {
            var types = Assembly.GetAssembly(typeof(ToolList)).DefinedTypes.Where(w => w.GetCustomAttribute<McpServerToolTypeAttribute>() != null).ToList();
            Tools = types.SelectMany(w => w.GetMethods().Where(v => v.GetCustomAttribute<McpServerToolAttribute>() != null)).ToList();

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
            EditorApplication.update += ProcessCommands;
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
                            System.Diagnostics.Debug.WriteLine($"[server <- client]: {JsonConvert.SerializeObject(body)}");

                        var stream = response.OutputStream;

                        try
                        {
                            switch (body?.Method)
                            {
                                case "initialize":
                                    await SendResponse(stream, await HandleInitializeRequest(body));
                                    break;

                                case "notifications/initialized":
                                    // No response needed for this notification
                                    break;

                                case "tools/list":
                                    await SendResponse(stream, await HandleToolListRequest(body));
                                    break;

                                case "tools/call":
                                    await SendResponse(stream, await HandleToolCallRequest(body));
                                    break;

                                case null:
                                    break;

                                default:
                                    Debug.LogWarning($"Unknown method: {body?.Method}");
                                    break;
                            }
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
            var result = await Handshake.CreateAsync();
            return new JsonRpcSuccessResponse<Handshake> { JsonRpc = "2.0", Id = request.Id, Result = result };
        }

        private static async Task<JsonRpcResponse> HandleToolListRequest(JsonRpcRequest<JObject> request)
        {
            var result = await ToolList.CreateAsync();
            return new JsonRpcSuccessResponse<ToolList> { JsonRpc = "2.0", Id = request.Id, Result = result };
        }

        private static async Task<JsonRpcResponse> HandleToolCallRequest(JsonRpcRequest<JObject> request)
        {
            TaskCompletionSource<JsonRpcResponse> source = new();

            lock (LockObj)
            {
                _queue[request.Id] = (request, source);
            }

            return await source.Task;
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


        private static async Task SendResponse(Stream stream, object obj)
        {
            var json = JsonConvert.SerializeObject(obj, SerializerSettings);
            System.Diagnostics.Debug.WriteLine($"[client <- server]: {json}");

            var data = Encoding.UTF8.GetBytes($"data: {json}\n\n");
            await stream.WriteAsync(data, 0, data.Length);
            await stream.FlushAsync();
        }

        private static JsonRpcRequest<T> GetRequest<T>(HttpListenerRequest request)
        {
            using var reader = new StreamReader(request.InputStream, Encoding.UTF8);
            var json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<JsonRpcRequest<T>>(json);
        }

        private static void ProcessCommands()
        {
            lock (LockObj)
            {
                foreach (var item in _queue)
                {
                    var key = item.Key;
                    var request = item.Value.Object;
                    var source = item.Value.TaskCompletionSource;
                    _queue.TryRemove(item.Key, out _);

                    var tool = Tools.FirstOrDefault(w => w.Name == request.Params["name"]?.ToString());
                    if (tool == null)
                    {
                        source.SetResult(new JsonRpcErrorResponse<ErrorAboutTool>
                        {
                            JsonRpc = "2.0",
                            Id = request.Id,
                            Error = new ErrorResponse<ErrorAboutTool>
                            {
                                Code = -32000,
                                Message = "Tool not found",
                                Data = new ErrorAboutTool { ToolName = request.Params["name"]?.ToString() }
                            }
                        });
                        continue;
                    }

                    var arguments = request.Params["arguments"];
                    var parameters = new List<object>();
                    foreach (var parameter in tool.GetParameters())
                    {
                        var argument = arguments?[parameter.Name];
                        if (argument == null && parameter.HasCustomAttribute<RequiredAttribute>())
                        {
                            source.SetResult(new JsonRpcErrorResponse<ErrorAboutTool>
                            {
                                JsonRpc = "2.0",
                                Id = request.Id,
                                Error = new ErrorResponse<ErrorAboutTool>
                                {
                                    Code = -32000,
                                    Message = $"Required argument '{parameter.Name}' is missing.",
                                    Data = new ErrorAboutTool { ToolName = tool.Name }
                                }
                            });
                            goto c;
                        }

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

                    try
                    {
                        var result = tool.Invoke(null, parameters.ToArray());
                        if (result is IErrorResult error)
                        {
                            source.SetResult(new JsonRpcErrorResponse<ErrorAboutTool>
                            {
                                JsonRpc = "2.0",
                                Id = request.Id,
                                Error = new ErrorResponse<ErrorAboutTool> { Code = -32000, Message = error.Error, Data = new ErrorAboutTool { ToolName = tool.Name } }
                            });
                        }
                        else
                        {
                            var contents = ToContent(result).ToArray();
                            source.SetResult(new JsonRpcSuccessResponse<CallToolResults> { JsonRpc = "2.0", Id = request.Id, Result = new CallToolResults { Content = contents } });
                        }
                    }
                    catch (Exception e)
                    {
                        source.SetResult(new JsonRpcErrorResponse<ErrorAboutTool> { JsonRpc = "2.0", Id = request.Id, Error = new ErrorResponse<ErrorAboutTool> { Code = -32000, Message = $"Tool execution failed: {e.Message}", Data = new ErrorAboutTool { ToolName = tool.Name } } });
                    }

                    c:
                    continue;
                }
            }
        }
    }
}
