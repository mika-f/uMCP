using System.IO;

using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions
{
    public class EmbeddedResourceContentResult : IToolResult
    {
        public EmbeddedResourceContentResult(string uri, string description)
        {
            Uri = uri;
            Description = description;
        }

        public string Uri { get; }
        public string Description { get; }

        public CallToolResultContentBase ToResponse()
        {
            using var sr = new StreamReader(Uri);

            return new CallToolResultEmbeddedResourceContent
            {
                Resource = new Resource
                {
                    Uri = Uri,
                    Title = Description,
                    MimeType = "text/plain", // Assuming text/plain for simplicity; adjust as needed.
                    Text = sr.ReadToEnd()
                }
            };
        }
    }
}
