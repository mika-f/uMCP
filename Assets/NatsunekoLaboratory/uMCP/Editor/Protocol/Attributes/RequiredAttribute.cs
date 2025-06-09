using System;

namespace NatsunekoLaboratory.uMCP.Protocol.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class RequiredAttribute : Attribute { }
}
