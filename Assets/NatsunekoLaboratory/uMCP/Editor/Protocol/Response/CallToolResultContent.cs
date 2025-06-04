using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Response
{
    public class CallToolResultContent
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
