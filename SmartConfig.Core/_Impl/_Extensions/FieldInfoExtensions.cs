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

#if NET40

        public static IEnumerable<T> GetCustomAttributes<T>(this FieldInfo fieldInfo, bool inherit = true) where T : Attribute
        {
            return fieldInfo.GetCustomAttributes(typeof (T), inherit).Cast<T>();
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this FieldInfo fieldInfo, bool inherit = true) where TAttribute : Attribute
        {
            return fieldInfo.GetCustomAttributes<TAttribute>(inherit).SingleOrDefault();
        }

#endif

        public static bool HasAttribute<T>(this FieldInfo fieldInfo, bool inherit = false) where T : Attribute
        {
#if NET40
            return fieldInfo.GetCustomAttributes(typeof(T), false).SingleOrDefault() != null;
#else
            return fieldInfo.GetCustomAttribute<T>(false) != null;
#endif
        }

        #region Constraint shortcuts

        public static IEnumerable<ConstraintAttribute> Contraints(this FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttributes<ConstraintAttribute>(false);
        }

        #endregion

        #region 

        public static bool IsNullable(this FieldInfo fieldInfo)
        {
            var isNullable =
                (fieldInfo.FieldType.IsValueType && fieldInfo.FieldType.IsNullable())
                || fieldInfo.GetCustomAttribute<OptionalAttribute>() != null;
            return isNullable;
        }

        public static bool IsOptional(this FieldInfo fieldInfo)
        {
            var isOptional = fieldInfo.GetCustomAttribute<OptionalAttribute>() != null;
            return isOptional;
        }

        #endregion
    }
}
