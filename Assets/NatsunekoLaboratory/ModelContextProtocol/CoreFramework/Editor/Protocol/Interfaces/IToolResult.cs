using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces
{
    public interface IToolResult
    {
        CallToolResultContentBase ToResponse();
    }
}