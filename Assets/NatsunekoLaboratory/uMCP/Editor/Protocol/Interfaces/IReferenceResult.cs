namespace NatsunekoLaboratory.uMCP.Protocol.Interfaces
{
    public interface IReferenceResult : IToolResult
    {
        string Reference { get; }

        string FileName { get; }

        string Description { get; }
    }
}
