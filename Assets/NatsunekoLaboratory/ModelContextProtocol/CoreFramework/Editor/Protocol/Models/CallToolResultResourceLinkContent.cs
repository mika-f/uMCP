using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models
{
    public class CallToolResultResourceLinkContent : CallToolResultContentBase
    {
        public CallToolResultResourceLinkContent()
        {
            Type = "resource_link";
        }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }
    }
}
