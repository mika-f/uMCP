using System.ComponentModel;

using NatsunekoLaboratory.uMCP.Protocol.Attributes;

namespace NatsunekoLaboratory.uMCP.Models
{
    public class Transform
    {
        [Required]
        [Description("position of Transform")]
        public Vector3 Position { get; set; }

        [Required]
        [Description("rotation of Transform (eulerAngles)")]
        public Vector3 Rotation { get; set; }

        [Required]
        [Description("scale of Transform")]
        public Vector3 Scale { get; set; }
    }
}
