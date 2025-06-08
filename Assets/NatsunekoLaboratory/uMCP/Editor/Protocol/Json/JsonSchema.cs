using System.Collections.Generic;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NatsunekoLaboratory.uMCP.Protocol.Json
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
    }
}
