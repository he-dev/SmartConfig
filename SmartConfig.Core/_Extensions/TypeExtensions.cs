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

        public static bool IsNullable(this Type type)
        {
            var isNullable =
                type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            return isNullable;
        }

        public static bool IsIEnumerable(this Type type)
        {
            var isIEnumerable =
                type != typeof(string)
                && type.GetInterfaces()
                .Any(t => t.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return isIEnumerable;
        }

        public static bool IsList(this Type type)
        {
            var isList =
                type != typeof(string)
                && type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(List<>);
            return isList;
        }

        public static bool IsDictionary(this Type type)
        {
            var isList =
                type != typeof(string)
                && type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
            return isList;
        }

        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), false).Any();
        }

//        public static T GetCustomAttribute<T>(this Type type, bool inherit = false) where T : Attribute
//        {
//#if NET40
//            return (T)type.GetCustomAttributes(typeof(T), inherit).SingleOrDefault();
//#else
//            return type.GetCustomAttributes(inherit).OfType<T>().SingleOrDefault();
//#endif
//        }
    }
}
