using System;
using System.IO;

using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions
{
    public class ResourceLinkContentResult : IToolResult
    {
        public ResourceLinkContentResult(string uri, string description)
        {
            Uri = uri;
            Name = Path.GetFileName(new Uri(uri).LocalPath);
            Description = description;
        }

        public string Uri { get; }
        public string Name { get; }
        public string Description { get; }

        public CallToolResultContentBase ToResponse()
        {
            return new CallToolResultResourceLinkContent
            {
                Name = Name,
                Uri = Uri,
                Description = Description,
                MimeType = "text/plain" // Assuming text/plain for simplicity; adjust as needed.
            };
        }
    }
}
