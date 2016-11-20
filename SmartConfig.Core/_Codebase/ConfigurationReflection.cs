using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Reusable;
using Reusable.Data.DataAnnotations;
using Reusable.Extensions;
using Reusable.Validations;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;

namespace SmartConfig
{
    internal static class ConfigurationReflection
    {
        internal static List<SettingProperty> GetSettingProperties(this Type configType)
        {
            var types = configType.NestedTypes(type => !type.HasAttribute<IgnoreAttribute>());

            var settingProperties =
                types.Select(type => type.GetProperties(BindingFlags.Public | BindingFlags.Static))
                .SelectMany(properties => properties)
                .Where(property => !property.HasAttribute<IgnoreAttribute>())
                .Select(property => new SettingProperty(property))
                .ToList();

            return settingProperties;
        }

        public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return memberInfo.GetCustomAttributes<T>(false).Any();
        }

        public static string GetCustomNameOrDefault(this MemberInfo member)
        {
            return member.Validate(nameof(member)).IsNotNull().Value.GetCustomAttribute<RenameAttribute>()?.Name ?? member.Name;
        }

        public static IEnumerable<string> GetSettingPath(this PropertyInfo propertyInfo)
        {
            var path = new LinkedList<string>();

            var member = (MemberInfo)propertyInfo;
            var smartConfigAttribute = (SmartConfigAttribute)null;
            do
            {
                path.AddFirst(member.GetCustomNameOrDefault());
                member = member.DeclaringType;
            } while (member != null && (smartConfigAttribute = member.GetCustomAttribute<SmartConfigAttribute>()) == null);

            // This should never happen because the type is already checked.
            if (smartConfigAttribute == null) { throw new SmartConfigAttributeNotFoundException(propertyInfo); }

            // Add config name if available.
            if (!string.IsNullOrEmpty(smartConfigAttribute.Name) && smartConfigAttribute.NameTarget == ConfigurationNameTarget.Path)
            {
                path.AddFirst(smartConfigAttribute.Name);
            }

            return path;
        }
    }
}
