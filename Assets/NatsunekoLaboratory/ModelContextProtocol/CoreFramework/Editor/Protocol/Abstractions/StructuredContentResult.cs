using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models;

using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions
{
    public class StructuredContentResult<T> : IStructuredToolResult
    {
        public StructuredContentResult(T content)
        {
            StructuredContent = content;
        }

        public T StructuredContent { get; }

        public CallToolResultContentBase ToResponse()
        {
            return new CallToolResultTextContent { Text = JsonConvert.SerializeObject(StructuredContent) };
        }

        public object ToStructuredResponse()
        {
            return StructuredContent;
        }
    }
}
