using System;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes.Validators
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class RequiredAttribute : Attribute { }
}
