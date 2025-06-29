using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models;

using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions
{
    public class ErrorStructuredContentResult : IStructuredToolResult
    {
        public ErrorStructuredContentResult(object error)
        {
            Error = error;
        }

        public object Error { get; }

        public CallToolResultContentBase ToResponse()
        {
            return new CallToolResultTextContent { Text = JsonConvert.SerializeObject(Error), Type = "text" };
        }

        public object ToStructuredResponse()
        {
            return Error;
        }
    }
}
