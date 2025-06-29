using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Models;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Response;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions
{
    public class AudioContentResult : IToolResult
    {
        public AudioContentResult(string audio)
        {
            Path = audio;
        }

        public string Path { get; }

        public CallToolResultContentBase ToResponse()
        {
            return new CallToolResultAudioContent
            {
                Data = MediaEncoder.GetMimeTypeFromFile(Path),
                MimeType = MediaEncoder.GetMimeTypeFromFile(Path)
            };
        }
    }
}
