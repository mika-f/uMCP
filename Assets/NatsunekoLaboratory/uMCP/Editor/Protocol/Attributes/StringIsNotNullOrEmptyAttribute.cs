using System;

using JetBrains.Annotations;

using NatsunekoLaboratory.uMCP.Protocol.Abstractions;
using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

namespace NatsunekoLaboratory.uMCP.Protocol.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal class StringIsNotNullOrEmptyAttribute : ValidatedParameterAttributeBase
    {
        public override bool Validate(object obj, [CanBeNull] out IErrorResult error)
        {
            if (obj is not string str)
            {
                error = new ErrorResult("parameter is not a string.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                error = new ErrorResult("string cannot be null or empty.");
                return false;
            }

            error = null;
            return true;
        }
    }
}
