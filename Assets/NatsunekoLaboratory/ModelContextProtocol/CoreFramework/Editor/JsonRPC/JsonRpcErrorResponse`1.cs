using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Response;

using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.JsonRPC
{
    public class JsonRpcErrorResponse<T> : JsonRpcResponse
    {
        [JsonProperty("error")]
        public ProtocolErrorResponse<T> Error { get; set; }
    }
}
