using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Extensions
{
    public static class BitmapExtensions
    {
        public static string ToBase64String(this Bitmap bitmap, ImageFormat format)
        {
            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, format);
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }
}
