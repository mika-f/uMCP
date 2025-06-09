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
            var schema = new JsonSchema
            {
                Type = t switch
                {
                    not null when typeof(string) == t => JsonSchemaType.String,
                    not null when typeof(int) == t => JsonSchemaType.Integer,
                    not null when typeof(long) == t => JsonSchemaType.Integer,
                    not null when typeof(float) == t => JsonSchemaType.Number,
                    not null when typeof(double) == t => JsonSchemaType.Number,
                    not null when typeof(bool) == t => JsonSchemaType.Boolean,
                    not null when t.IsEnum => JsonSchemaType.String,
                    not null when t.IsArray => JsonSchemaType.Array,
                    not null when t.IsClass => JsonSchemaType.Object,
                    _ => throw new ArgumentOutOfRangeException()
                }
            };

            if (t.IsEnum) { }
            else
            {
                switch (schema.Type)
                {
                    case JsonSchemaType.Array:
                    {
                        schema.Items = GenerateFromType(t.GenericTypeArguments[0]);
                        break;
                    }

                    case JsonSchemaType.Object:
                    {
                        schema.Properties = new Dictionary<string, JsonSchema>();
                        foreach (var property in t.GetProperties())
                        {
                            var name = property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? property.Name;
                            var innerSchema = GenerateFromType(property.PropertyType);
                            schema.Properties.Add(name, innerSchema);

                            if (property.GetCustomAttribute<DescriptionAttribute>() != null)
                            {
                                var description = property.GetCustomAttribute<DescriptionAttribute>();
                                innerSchema.Description = description.Description;
                            }

                            if (property.HasCustomAttribute<RequiredAttribute>())
                                schema.Required.Add(name);
                        }

                        break;
                    }
                }
            }

            return schema;
        }

        private static JsonSchema GenerateFromParameterInfo(ParameterInfo pi)
        {
            var schema = GenerateFromType(pi.ParameterType);
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

                            var attribute = w.GetCustomAttribute<McpServerToolAttribute>();
                            return new Tool
                            {
                                Name = w.Name,
                                Description = w.GetCustomAttribute<DescriptionAttribute>().Description ?? "",
                                InputSchema = schema,
                                Annotations = new Dictionary<string, object>
                                {
                                    { "title", attribute.Name },
                                    { "readOnlyHint", attribute.Readonly },
                                    { "destructiveHint", attribute.Destructive },
                                    { "idempotentHint", attribute.Idempotent },
                                    { "openWorldHint", attribute.OpenWorld }
                                    // { "costly", w.GetCustomAttribute<McpServerToolAttribute>().Costly },
                                    // { "requiresHumanApproval", w.GetCustomAttribute<McpServerToolAttribute>().RequiresHumanApproval }
                                }
                            };
                        })
                        .ToList()
            };
        }
    }
}
