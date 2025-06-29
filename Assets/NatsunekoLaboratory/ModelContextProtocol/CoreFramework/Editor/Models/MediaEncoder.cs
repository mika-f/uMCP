using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Models
{
    public static class MediaEncoder
    {
        private static readonly Dictionary<byte[], string> MimeTypes = new()
        {
            // Image
            { new byte[] { 0xFF, 0xD8, 0xFF }, "image/jpeg" }, //      JPG
            { new byte[] { 0x89, 0x50, 0x4E, 0x47 }, "image/png" }, // PNG
            { new byte[] { 0x47, 0x49, 0x46, 0x38 }, "image/gif" }, // GIF
            { new byte[] { 0x42, 0x4D }, "image/bmp" }, //             BMP

            // Audio
            { new byte[] { 0x49, 0x44, 0x33 }, "audio/mpeg" }, //      MP3
            { new byte[] { 0x52, 0x49, 0x46, 0x46 }, "audio/wav" }, // WAV
            { new byte[] { 0x66, 0x74, 0x79, 0x70 }, "audio/aac" }, // AAC
            { new byte[] { 0x4F, 0x67, 0x67, 0x53 }, "audio/ogg" } //  OGG
        };

        public static string ReadFromFile(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                throw new ArgumentException("Invalid file path.", nameof(path));

            var fileBytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(fileBytes);
        }

        public static string GetMimeTypeFromFile(string path)
        {
            var header = new byte[4];
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            stream.Read(header, 0, header.Length);

            foreach (var kvp in MimeTypes.Where(kvp => header.Length >= kvp.Key.Length && kvp.Key.SequenceEqual(header.Take(kvp.Key.Length))))
                return kvp.Value;

            return "application/octet-stream";
        }
    }
}
