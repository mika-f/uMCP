using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Json
{
    public class JsonRpcResponse
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public class JsonRpcSuccessResponse<T> : JsonRpcResponse
    {
        [JsonProperty("result")]
        public T Result { get; set; }
    }

    public class JsonRpcErrorResponse<T> : JsonRpcResponse
    {
        [JsonProperty("error")]
        public ErrorResponse<T> Error { get; set; }
    }

    public class ErrorResponse<T>
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
