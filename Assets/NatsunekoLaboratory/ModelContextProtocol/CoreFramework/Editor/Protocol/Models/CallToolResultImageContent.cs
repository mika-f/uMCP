using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models
{
    public class CallToolResultImageContent : CallToolResultContentBase
    {
        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        public CallToolResultImageContent()
        {
            Type = "image";
        }
    }
}
