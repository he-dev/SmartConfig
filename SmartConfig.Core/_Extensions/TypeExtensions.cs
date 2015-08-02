using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public static class TypeExtensions
    {
        public static bool IsStatic(this Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }

#if NET40

        public static IEnumerable<T> GetCustomAttributes<T>(this Type type, bool inherit = false) where T : Attribute
        {
            return type.GetCustomAttributes(typeof (T), inherit).Cast<T>();
        }


        public static TAttribute GetCustomAttribute<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute
        {
            return type.GetCustomAttributes<TAttribute>(inherit).SingleOrDefault();
        }

#endif
    }
}
