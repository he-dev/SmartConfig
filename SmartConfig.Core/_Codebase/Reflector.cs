using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartConfig.DataAnnotations;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig
{
    internal static class Reflector
    {
        // gets either custom or default member name
        public static string GetCustomNameOrDefault(this MemberInfo member)
        {
            member.Validate(nameof(member)).IsNotNull();
            return member.GetCustomAttribute<RenameAttribute>()?.Name ?? member.Name;
        }

        public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return memberInfo.GetCustomAttributes<T>(false).Any();
        }

        public static List<string> GetSettingPath(this PropertyInfo propertyInfo)
        {
            var path = new List<string> { propertyInfo.GetCustomNameOrDefault() };

            var type = propertyInfo.DeclaringType;

            while (type != null && !type.HasAttribute<SmartConfigAttribute>())
            {
                path.Add(type.GetCustomNameOrDefault());
                type = type.DeclaringType;
                if (type == null)
                {
                    throw new SmartConfigAttributeNotFoundException
                    {
                        Property = propertyInfo.Name
                    };
                }
            }

            // add config name if available
            path.Add(type.GetCustomAttribute<SmartConfigAttribute>()?.Name);
            path.Reverse();
            return path;
        }
    }
}
