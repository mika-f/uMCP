using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models
{
    public class CallToolResultContentBase
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
