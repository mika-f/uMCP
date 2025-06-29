using System;

using JetBrains.Annotations;

using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes.Validators
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class StringIsNotNullOrEmptyAttribute : ValidatedParameterAttributeBase
    {
        public override bool Validate(object obj, [CanBeNull] out ErrorContentResult error)
        {
            if (obj is not string str)
            {
                error = new ErrorContentResult("parameter is not a string.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                error = new ErrorContentResult("string cannot be null or empty.");
                return false;
            }

            error = null;
            return true;
        }
    }
}
