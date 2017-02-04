using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Reusable.Fuse;
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
                var name = GetNameOrDefault(parent);
                path.AddFirst(name);
            }
            return path;
        }

        public static string GetNameOrDefault(this MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttribute<SettingNameAttribute>()?.Name ?? memberInfo.Name;
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
