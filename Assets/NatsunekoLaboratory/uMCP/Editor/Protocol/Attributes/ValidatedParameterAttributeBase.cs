using System;

using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

namespace NatsunekoLaboratory.uMCP.Protocol.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ValidatedParameterAttributeBase : Attribute
    {
        public abstract bool Validate(object obj, out IErrorResult error);
    }
}
