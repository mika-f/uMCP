using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.JsonRPC
{
    public class JsonRpcSuccessResponse<T> : JsonRpcResponse
    {
        [JsonProperty("result")]
        public T Result { get; set; }
    }
}
