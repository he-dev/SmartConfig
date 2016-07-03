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
    }
}
