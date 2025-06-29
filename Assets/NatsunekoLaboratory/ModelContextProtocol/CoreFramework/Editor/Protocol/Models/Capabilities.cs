using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models
{
    public class Capabilities
    {
        [JsonProperty("listChanged")]
        public bool ListChanged { get; set; }
    }
}
