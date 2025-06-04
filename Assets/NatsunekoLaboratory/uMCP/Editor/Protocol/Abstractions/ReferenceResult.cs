using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

namespace NatsunekoLaboratory.uMCP.Protocol.Abstractions
{
    public class ReferenceResult : IReferenceResult
    {
        public ReferenceResult(string reference, string fileName, string description)
        {
            Reference = reference;
            FileName = fileName;
            Description = description;
        }

        public string Reference { get; }
        public string FileName { get; }
        public string Description { get; }
    }
}
