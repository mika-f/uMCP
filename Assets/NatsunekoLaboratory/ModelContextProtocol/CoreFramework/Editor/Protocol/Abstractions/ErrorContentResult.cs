using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions
{
    public class ErrorContentResult : IToolResult
    {
        public ErrorContentResult(string error)
        {
            Error = error;
        }

        public string Error { get; }

        public CallToolResultContentBase ToResponse()
        {
            return new CallToolResultTextContent { Text = Error, Type = "text" };
        }
    }
}
