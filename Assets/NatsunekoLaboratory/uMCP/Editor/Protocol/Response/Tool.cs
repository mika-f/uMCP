using System.Collections.Generic;

using NatsunekoLaboratory.uMCP.Protocol.Json;

using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Response
{
    public class Tool
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("inputSchema")]
        public JsonSchema InputSchema { get; set; }

        [JsonProperty("annotations")]
        public Dictionary<string, object> Annotations { get; set; }
    }
}
