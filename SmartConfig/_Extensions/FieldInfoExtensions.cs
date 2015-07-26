using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig
{
    internal static class FieldInfoExtensions
    {
        public static bool HasAttribute<T>(this FieldInfo fieldInfo, bool inherit) where T : Attribute
        {
            return fieldInfo.GetCustomAttribute<T>(inherit) != null;
        }

#if NET40
        public static bool GetCustomAttributes<T>(this FieldInfo fieldInfo, bool inherit) where T : Attribute
        {
            return fieldInfo.GetCustomAttributes(typeof(T), inherit).SingleOrDefault() != null;
        }    
#endif
    }
}
