using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Response
{
    public class ProtocolErrorResponse<T>
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
