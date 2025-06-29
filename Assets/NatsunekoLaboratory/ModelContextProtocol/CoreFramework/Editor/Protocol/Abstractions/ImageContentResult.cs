using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Models;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions
{
    public class ImageContentResult : IToolResult
    {
        public ImageContentResult(string image)
        {
            Path = image;
        }

        private string Path { get; }

        public CallToolResultContentBase ToResponse()
        {
            return new CallToolResultImageContent
            {
                Data = MediaEncoder.ReadFromFile(Path),
                MimeType = MediaEncoder.GetMimeTypeFromFile(Path)
            };
        }
    }
}