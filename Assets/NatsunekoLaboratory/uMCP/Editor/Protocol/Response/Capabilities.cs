using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Response
{
    public class Capabilities
    {
        [JsonProperty("listChanged")]
        public bool ListChanged { get; set; }

        [JsonProperty("subscribe")]
        public bool? Subscribe { get; set; }
    }
}
