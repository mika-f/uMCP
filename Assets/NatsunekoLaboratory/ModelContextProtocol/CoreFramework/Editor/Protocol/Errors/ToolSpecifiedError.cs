
using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Errors
{
    public class ToolSpecifiedError
    {
        [JsonProperty("toolName")]
        public string ToolName { get; set; }
    }
}