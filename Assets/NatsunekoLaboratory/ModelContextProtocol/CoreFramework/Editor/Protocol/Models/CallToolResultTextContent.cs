using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models
{
    public class CallToolResultTextContent : CallToolResultContentBase
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        public CallToolResultTextContent()
        {
            Type = "text";
        }
    }
}
