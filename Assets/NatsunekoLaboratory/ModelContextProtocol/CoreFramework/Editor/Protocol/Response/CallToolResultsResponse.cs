using JetBrains.Annotations;

using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models;

using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Response
{
    public class CallToolResultsResponse
    {
        [JsonProperty("content")]
        public CallToolResultContentBase[] Content { get; set; }

        [JsonProperty("structuredContent")]
        [CanBeNull]
        public object StructuredContent { get; set; }

        [JsonProperty("isError")]
        public bool? IsError { get; set; }
    }
}
