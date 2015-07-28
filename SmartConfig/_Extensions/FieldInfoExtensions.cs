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


        public static TAttribute GetCustomAttribute<TAttribute>(this FieldInfo fieldInfo, bool inherit) where TAttribute : Attribute
        {
            return fieldInfo.GetCustomAttributes<TAttribute>(inherit).SingleOrDefault();
        }   

#endif

        #region Constraint shortcuts

        public static IEnumerable<ValueContraintAttribute> Contraints(this FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttributes<ValueContraintAttribute>(true);
        }

        public static bool IsNullable(this FieldInfo fieldInfo)
        {
            var isNullable =
                (fieldInfo.FieldType.IsValueType && fieldInfo.FieldType.IsNullable())
                || fieldInfo.Contraints().OfType<NullableAttribute>().SingleOrDefault() != null;
            return isNullable;
        }

        public static bool IsOptional(this FieldInfo fieldInfo)
        {
            var allowsNull = fieldInfo.Contraints().OfType<OptionalAttribute>().SingleOrDefault() != null;
            return allowsNull;
        }


        #endregion
    }
}
