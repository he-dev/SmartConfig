using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartConfig.Data.Annotations;

namespace SmartConfig
{
    // Provides methods for reflecting the configuration type.
    internal static class ConfigurationReflection
    {
        public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return memberInfo.GetCustomAttributes<T>(false).Any();
        }

        public static IEnumerable<string> GetPath(this PropertyInfo propertyInfo)
        {
            var path = new LinkedList<string>();

            foreach (var parent in propertyInfo.Parents())
            {
                var name = GetSettingName(parent);
                //if (string.IsNullOrEmpty(name)) continue;
                path.AddFirst(name);
            }
            return path;
        }

        public static string GetSettingName(this MemberInfo memberInfo)
        {
            var name = memberInfo.GetCustomAttribute<SettingNameAttribute>()?.ToString() ?? memberInfo.Name;

            switch (name)
            {
                case SettingName.Default: return memberInfo.Name;
                case SettingName.Unset: return null;
                default: return name;
            }

        }

        private static IEnumerable<MemberInfo> Parents(this MemberInfo member)
        {
            do
            {
                yield return member;
                if (member.HasAttribute<SmartConfigAttribute>()) yield break;
                member = member.DeclaringType;
            } while (member != null);
        }
    }
}
