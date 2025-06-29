using System.Threading.Tasks;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models
{
    public class ServerCapabilities
    {
        [JsonProperty("logging")]
        public object Logging { get; set; }

        [JsonProperty("tools")]
        [CanBeNull]
        public Capabilities Tools { get; set; }

        [JsonProperty("prompts")]
        [CanBeNull]
        public Capabilities Prompts { get; set; }

        [JsonProperty("resources")]
        [CanBeNull]
        public Capabilities Resources { get; set; }

        public static async Task<ServerCapabilities> CreateAsync()
        {
            return new ServerCapabilities
            {
                Tools = new Capabilities { ListChanged = true },
                Resources = new Capabilities { ListChanged = true },
                Prompts = new Capabilities { ListChanged = false }
            };
        }
    }
}