using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Response
{
    public class CallToolReferenceContent : CallToolResultContent
    {
        [JsonProperty("resource")]
        public Resource Resource { get; set; }

        public CallToolReferenceContent()
        {
            Type = "resource";
        }
    }
}
