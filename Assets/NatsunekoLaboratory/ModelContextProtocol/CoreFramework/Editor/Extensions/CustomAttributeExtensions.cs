using System;
using System.Linq;
using System.Reflection;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Extensions
{
    public static class CustomAttributeExtensions
    {
        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider t) where T : Attribute
        {
            return t.GetCustomAttributes(typeof(T), false).FirstOrDefault() != null;
        }

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider t) where T : Attribute
        {
            return (T)t.GetCustomAttributes(typeof(T), false).FirstOrDefault();
        }
    }
}
