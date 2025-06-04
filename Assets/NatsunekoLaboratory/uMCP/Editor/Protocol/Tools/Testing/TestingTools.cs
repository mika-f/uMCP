using System;
using System.ComponentModel;

using NatsunekoLaboratory.uMCP.Protocol.Abstractions;
using NatsunekoLaboratory.uMCP.Protocol.Attributes;
using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

namespace NatsunekoLaboratory.uMCP.Protocol.Tools.Testing
{
    [McpServerToolType]
    public class TestingTools
    {
        [McpServerTool]
        [Description("generate a GUID")]
        public static ITextResult GetGuid()
        {
            return new TextResult(Guid.NewGuid().ToString());
        }

        [McpServerTool(Readonly = true)]
        [Description("echo input")]
        public static ITextResult EchoInput([Description("echo input value")] [Required] string input)
        {
            return new TextResult(input);
        }
    }
}
