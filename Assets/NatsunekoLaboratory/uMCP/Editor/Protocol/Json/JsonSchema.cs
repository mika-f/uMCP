using System.Collections.Generic;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Json
{
    public class JsonSchema
    {
        [JsonProperty("type")]
        public JsonSchemaType Type { get; set; }

        [JsonProperty("properties")]
        [CanBeNull]
        public Dictionary<string, JsonSchema> Properties { get; set; }

        [JsonProperty("items")]
        [CanBeNull]
        public List<JsonSchema> Items { get; set; }

        [JsonProperty("description")]
        [CanBeNull]
        public string Description { get; set; }

        [JsonProperty("required")]
        public List<string> Required { get; set; } = new();
    }
}
