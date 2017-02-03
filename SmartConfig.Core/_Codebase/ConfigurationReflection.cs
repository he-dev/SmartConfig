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
                var name = default(string);
                if (TryGetName(parent, out name))
                {
                    path.AddFirst(name);
                }
                else
                {
                    break;
                }
            }          
            return path;
        }

        private static bool TryGetName(MemberInfo memberInfo, out string name)
        {
            var smartConfigAttribute = memberInfo.GetCustomAttribute<SmartConfigAttribute>();
            if (smartConfigAttribute != null)
            {
                return !string.IsNullOrEmpty(name = smartConfigAttribute.Name);
            }

            name = memberInfo.GetCustomAttribute<RenameAttribute>()?.Name ?? memberInfo.Name;
            return true;
        }
        
        public static IEnumerable<MemberInfo> Parents(this PropertyInfo propertyInfo)
        {
            var member = (MemberInfo)propertyInfo;
            do
            {
                yield return member;
                member = member.DeclaringType;
            } while (member != null);            
        }
    }
}
