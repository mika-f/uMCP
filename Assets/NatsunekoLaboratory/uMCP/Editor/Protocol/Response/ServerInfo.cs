using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Response
{
    public class ServerInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
