using System.Drawing;

using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

namespace NatsunekoLaboratory.uMCP.Protocol.Abstractions
{
    public class ImageResult : IImageResult
    {
        public ImageResult(Bitmap image)
        {
            Image = image;
        }

        public Bitmap Image { get; }
    }
}