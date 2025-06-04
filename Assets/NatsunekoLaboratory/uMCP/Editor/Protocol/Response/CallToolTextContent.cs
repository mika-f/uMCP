using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Response
{
    public class CallToolTextContent : CallToolResultContent
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        public CallToolTextContent()
        {
            Type = "text";
        }
    }
}
