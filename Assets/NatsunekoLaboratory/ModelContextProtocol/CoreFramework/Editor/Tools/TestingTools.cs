using System;
using System.ComponentModel;

using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes.Validators;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Tools
{
    [McpServerToolType]
    public class TestingTools
    {
        [McpServerTool]
        [Description("generate a GUID")]
        public static IToolResult GetGuid()
        {
            return new TextContentResult(Guid.NewGuid().ToString());
        }

        [McpServerTool(Readonly = true)]
        [Description("echo input")]
        public static IToolResult EchoInput([Description("echo input value")] [Required] string input)
        {
            return new TextContentResult(input);
        }
    }
}
