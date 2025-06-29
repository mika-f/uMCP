using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using JetBrains.Annotations;

using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes.Validators;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Extensions;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.JsonSchema
{
    public class JsonSchema
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public JsonSchemaType Type { get; set; }

        [JsonProperty("properties")]
        [CanBeNull]
        public Dictionary<string, JsonSchema> Properties { get; set; }

        [JsonProperty("items")]
        [CanBeNull]
        public JsonSchema Items { get; set; }

        [JsonProperty("description")]
        [CanBeNull]
        public string Description { get; set; } = "";

        [JsonProperty("required")]
        public List<string> Required { get; set; } = new();


        public static JsonSchema FromType(Type t)
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
                        schema.Items = FromType(t.GenericTypeArguments[0]);
                        break;
                    }

                    case JsonSchemaType.Object:
                    {
                        schema.Properties = new Dictionary<string, JsonSchema>();
                        foreach (var property in t.GetProperties())
                        {
                            var name = property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? property.Name;
                            var innerSchema = FromType(property.PropertyType);
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

        public static JsonSchema FromType<T>()
        {
            return FromType(typeof(T));
        }

        public static JsonSchema FromParameterInfo(ParameterInfo pi)
        {
            var schema = FromType(pi.ParameterType);
            if (pi.GetCustomAttribute<DescriptionAttribute>() != null)
            {
                var description = pi.GetCustomAttribute<DescriptionAttribute>();
                schema.Description = description.Description;
            }

            return schema;
        }

        public static JsonSchema FromMethodInfo(MethodInfo mi)
        {
            var schema = new JsonSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, JsonSchema>()
            };

            foreach (var parameter in mi.GetParameters())
            {
                var parameterSchema = FromParameterInfo(parameter);
                schema.Properties.Add(parameter.Name, parameterSchema);

                if (parameter.HasCustomAttribute<RequiredAttribute>())
                    schema.Required.Add(parameter.Name);
            }

            return schema;
        }
    }
}
