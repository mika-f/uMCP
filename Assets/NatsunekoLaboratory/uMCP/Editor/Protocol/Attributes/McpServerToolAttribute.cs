using System;

using JetBrains.Annotations;

namespace NatsunekoLaboratory.uMCP.Protocol.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class McpServerToolAttribute : Attribute {
        [CanBeNull]
        public string Name { get; set; }

        public bool Readonly { get; set; } = false;

        public bool Destructive { get; set; } = false;

        public bool Costly { get; set; } = false;

        public bool RequiresHumanApproval { get; set; } = false;
    }
}
