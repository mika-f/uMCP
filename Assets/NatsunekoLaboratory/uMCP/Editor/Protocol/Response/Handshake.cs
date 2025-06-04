using System.Threading.Tasks;

using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Response
{
    public class Handshake
    {
        [JsonProperty("protocolVersion")]
        public string ProtocolVersion { get; set; }

        [JsonProperty("capabilities")]
        public ServerCapabilities Capabilities { get; set; }

        [JsonProperty("serverInfo")]
        public ServerInfo ServerInfo { get; set; }

        [JsonProperty("instructions")]
        public string Instructions { get; set; }

        public static async Task<Handshake> CreateAsync()
        {
            return new Handshake
            {
                ProtocolVersion = "2025-03-26",
                Capabilities = await ServerCapabilities.CreateAsync(),
                ServerInfo = new ServerInfo
                {
                    Name = "Unity Animation MCP",
                    Version = "0.1.0-alpha.1"
                },
                Instructions = ""
            };
        }
    }
}
