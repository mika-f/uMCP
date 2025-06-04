using System;
using System.Linq;
using System.Reflection;

namespace NatsunekoLaboratory.uMCP.Extensions
{
    public static class CustomAttributeExtensions
    {
        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider t) where T : Attribute
        {
            return t.GetCustomAttributes(typeof(T), false).FirstOrDefault() != null;
        }
    }
}
