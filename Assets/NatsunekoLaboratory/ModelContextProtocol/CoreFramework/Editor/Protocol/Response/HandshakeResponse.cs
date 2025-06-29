using System.Threading.Tasks;

using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models;

using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Response
{
    public class HandshakeResponse
    {
        [JsonProperty("protocolVersion")]
        public string ProtocolVersion { get; set; }

        [JsonProperty("capabilities")]
        public ServerCapabilities Capabilities { get; set; }

        [JsonProperty("serverInfo")]
        public ServerInfo ServerInfo { get; set; }

        [JsonProperty("instructions")]
        public string Instructions { get; set; }

        public static async Task<HandshakeResponse> CreateAsync()
        {
            return new HandshakeResponse
            {
                ProtocolVersion = "2025-06-18",
                Capabilities = await ServerCapabilities.CreateAsync(),
                ServerInfo = new ServerInfo
                {
                    Name = "Unity MCP Framework",
                    Version = "1.0.0-alpha.1"
                },
                Instructions = ""
            };
        }
    }
}
