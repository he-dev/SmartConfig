using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartConfig.DataAnnotations;

namespace SmartConfig
{
    internal static class Reflector
    {
        // gets either custom or default member name
        public static string GetName(this MemberInfo member)
        {
            if (member == null) { throw new ArgumentNullException(nameof(member)); }

            return member.GetCustomAttribute<CustomNameAttribute>()?.Name ?? member.Name;
        }

        public static bool IsSmartConfigType(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var smartConfigAttribute = type.GetCustomAttribute<SmartConfigAttribute>();
            return smartConfigAttribute != null;
        }

        public static List<string> GetPropertyPath(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var path = new List<string> { propertyInfo.GetName() };

            var type = propertyInfo.DeclaringType;

            while (type != null && !type.IsSmartConfigType())
            {
                path.Add(type.GetName());

                type = type.DeclaringType;
                if (type == null)
                {
                    throw new SmartConfigAttributeNotFoundException
                    {
                        AffectedProperty = propertyInfo.PropertyType.FullName
                    };
                }
            }

            path.Reverse();
            return path;
        }        

        public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            if (memberInfo == null) { throw new ArgumentNullException(nameof(memberInfo)); }

            return memberInfo.GetCustomAttributes(typeof(T), false).Any();
        }
        

        //public static bool IsNullable(this Type memberInfo)
        //{
        //    var isNullable =
        //        memberInfo.IsGenericType
        //        && memberInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
        //    return isNullable;
        //}

        //public static bool IsIEnumerable(this Type memberInfo)
        //{
        //    var isIEnumerable =
        //        memberInfo != typeof(string)
        //        && memberInfo.GetInterfaces()
        //        .Any(t => t.IsGenericType && memberInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        //    return isIEnumerable;
        //}

        //public static bool IsList(this Type memberInfo)
        //{
        //    var isList =
        //        memberInfo != typeof(string)
        //        && memberInfo.IsGenericType
        //        && memberInfo.GetGenericTypeDefinition() == typeof(List<>);
        //    return isList;
        //}

        //public static bool IsDictionary(this Type memberInfo)
        //{
        //    var isList =
        //        memberInfo != typeof(string)
        //        && memberInfo.IsGenericType
        //        && memberInfo.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        //    return isList;
        //}



        //        public static T GetCustomAttribute<T>(this Type memberInfo, bool inherit = false) where T : Attribute
        //        {
        //#if NET40
        //            return (T)memberInfo.GetCustomAttributes(typeof(T), inherit).SingleOrDefault();
        //#else
        //            return memberInfo.GetCustomAttributes(inherit).OfType<T>().SingleOrDefault();
        //#endif
        //        }
    }
}
