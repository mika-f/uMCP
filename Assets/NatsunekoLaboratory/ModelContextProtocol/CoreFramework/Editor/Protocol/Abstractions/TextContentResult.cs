using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions
{
    public class TextContentResult : IToolResult
    {
        public TextContentResult(string text)
        {
            Text = text;
        }

        public string Text { get; }

        public CallToolResultContentBase ToResponse()
        {
            return new CallToolResultTextContent { Text = Text };
        }
    }
}
