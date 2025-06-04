using System.Drawing;

namespace NatsunekoLaboratory.uMCP.Protocol.Interfaces
{
    public interface IImageResult : IToolResult
    {
        Bitmap Image { get; }
    }
}