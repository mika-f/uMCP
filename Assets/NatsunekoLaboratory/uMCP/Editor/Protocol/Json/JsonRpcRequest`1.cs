using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Json
{
    public class JsonRpcRequest<T>
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public T Params { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
