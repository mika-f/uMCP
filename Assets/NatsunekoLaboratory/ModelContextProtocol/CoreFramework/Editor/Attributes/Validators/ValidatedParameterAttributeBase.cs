using System;

using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes.Validators
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ValidatedParameterAttributeBase : Attribute
    {
        public abstract bool Validate(object obj, out ErrorContentResult error);
    }
}
