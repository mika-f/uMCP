
using Newtonsoft.Json;

namespace NatsunekoLaboratory.uMCP.Protocol.Errors
{
    public class ErrorAboutTool
    {
        [JsonProperty("toolName")]
        public string ToolName { get; set; }
    }
}