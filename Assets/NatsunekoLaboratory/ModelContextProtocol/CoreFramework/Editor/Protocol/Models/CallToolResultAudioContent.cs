using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models
{
    public class CallToolResultAudioContent : CallToolResultContentBase
    {
        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        public CallToolResultAudioContent()
        {
            Type = "audio";
        }
    }
}
