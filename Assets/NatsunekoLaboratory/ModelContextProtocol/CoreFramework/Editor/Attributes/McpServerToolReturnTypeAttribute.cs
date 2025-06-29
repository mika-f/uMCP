using System;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes
{
    [AttributeUsage(AttributeTargets.ReturnValue)]
    public class McpServerToolReturnTypeAttribute : Attribute
    {
        public Type T { get; }

        public McpServerToolReturnTypeAttribute(Type t)
        {
            T = t;
        }
    }
}