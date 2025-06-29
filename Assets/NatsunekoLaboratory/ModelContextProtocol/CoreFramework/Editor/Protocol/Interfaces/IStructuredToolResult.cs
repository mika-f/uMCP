namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces
{
    public interface IStructuredToolResult : IToolResult
    {
        object ToStructuredResponse();
    }
}
