using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using NatsunekoLaboratory.uMCP.Extensions;
using NatsunekoLaboratory.uMCP.Protocol.Attributes;
using NatsunekoLaboratory.uMCP.Protocol.Json;

using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Response
{
    public class ToolList
    {
        [JsonProperty("tools")]
        public List<Tool> Tools { get; set; }

        private static JsonSchema GenerateFromType(Type t)
        {
            return new JsonSchema();
        }

        private static JsonSchema GenerateFromParameterInfo(ParameterInfo pi)
        {
            var schema = new JsonSchema
            {
                Type = pi.ParameterType switch
                {
                    not null when typeof(string) == pi.ParameterType => JsonSchemaType.String,
                    not null when typeof(int) == pi.ParameterType => JsonSchemaType.Integer,
                    not null when typeof(long) == pi.ParameterType => JsonSchemaType.Integer,
                    not null when typeof(float) == pi.ParameterType => JsonSchemaType.Number,
                    not null when typeof(double) == pi.ParameterType => JsonSchemaType.Number,
                    not null when typeof(bool) == pi.ParameterType => JsonSchemaType.Boolean,
                    not null when pi.ParameterType.IsEnum => JsonSchemaType.String,
                    not null when pi.ParameterType.IsArray => JsonSchemaType.Array,
                    _ => throw new ArgumentOutOfRangeException()
                }
            };

            if (pi.ParameterType.IsEnum) { }

            if (pi.ParameterType.IsArray)
                schema.Items = GenerateFromType(pi.ParameterType.GenericTypeArguments[0]);

            if (pi.GetCustomAttribute<DescriptionAttribute>() != null)
            {
                var description = pi.GetCustomAttribute<DescriptionAttribute>();
                schema.Description = description.Description;
            }

            return schema;
        }

        public static async Task<ToolList> CreateAsync(List<MethodInfo> tools)
        {
            return new ToolList
            {
                Tools = tools
                        .Select(w =>
                        {
                            var schema = new JsonSchema
                            {
                                Type = JsonSchemaType.Object,
                                Properties = new Dictionary<string, JsonSchema>()
                            };

                            foreach (var parameter in w.GetParameters())
                            {
                                var s = GenerateFromParameterInfo(parameter);

                                schema.Properties.Add(parameter.Name, s);

                                if (parameter.HasCustomAttribute<RequiredAttribute>())
                                    schema.Required.Add(parameter.Name);
                            }


                            return new Tool
                            {
                                Name = w.GetCustomAttribute<McpServerToolAttribute>().Name ?? w.Name,
                                Description = w.GetCustomAttribute<DescriptionAttribute>().Description ?? "",
                                InputSchema = schema
                            };
                        })
                        .ToList()
            };
        }
    }
}
