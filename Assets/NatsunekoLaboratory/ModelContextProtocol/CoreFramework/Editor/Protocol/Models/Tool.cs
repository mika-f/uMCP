using System.Collections.Generic;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models
{
    public class Tool
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("inputSchema")]
        public JsonSchema.JsonSchema InputSchema { get; set; }

        [JsonProperty("outputSchema")]
        [CanBeNull]
        public JsonSchema.JsonSchema OutputSchema { get; set; }

        [JsonProperty("annotations")]
        public Dictionary<string, object> Annotations { get; set; }
    }
}
