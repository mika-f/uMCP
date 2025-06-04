using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Response
{
    public class CallToolResults
    {
        [JsonProperty("content")]
        public CallToolResultContent[] Content { get; set; }
    }
}
