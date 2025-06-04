using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Response
{
    public class CallToolImageContent : CallToolResultContent
    {
        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        public CallToolImageContent()
        {
            Type = "image";
        }
    }
}
