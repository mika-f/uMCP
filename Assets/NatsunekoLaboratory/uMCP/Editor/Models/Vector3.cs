using System.ComponentModel;

using NatsunekoLaboratory.uMCP.Protocol.Attributes;

namespace NatsunekoLaboratory.uMCP.Models
{
    [Description("Vector3D represents UnityEngine.Vector3 for in/out with JSON")]
    public class Vector3
    {
        [Required]
        [Description("X component of the vector")]
        public float X { get; set; }

        [Required]
        [Description("Y component of the vector")]
        public float Y { get; set; }

        [Required]
        [Description("Z component of the vector")]
        public float Z { get; set; }

        public UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3(X, Y, Z);
        }
    }
}
