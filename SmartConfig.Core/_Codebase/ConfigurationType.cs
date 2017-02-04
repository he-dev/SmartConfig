using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reusable;
using Reusable.Data.Annotations;
using SmartConfig.Data;
using SmartConfig.Data.Annotations;

namespace SmartConfig
{
    // 
    internal static class ConfigurationType
    {
        internal static IEnumerable<SettingProperty> GetSettingProperties(Type configType)
        {
            var includeConfigName = IncludeConfigName(configType);

            var nameCount = includeConfigName ? 0 : 1;

            var types = configType.NestedTypes(type => !type.HasAttribute<IgnoreAttribute>());

            var settingProperties =
                types.Select(type => type.GetProperties(BindingFlags.Public | BindingFlags.Static))
                    .SelectMany(properties => properties)
                    .Where(property => !property.HasAttribute<IgnoreAttribute>())
                    .Select(property => new SettingProperty(
                        property, 
                        new SettingPath(property.GetPath().Skip(nameCount))
                    ));

            return settingProperties;
        }

        private static bool IncludeConfigName(MemberInfo configType)
        {
            var smartConfig = configType.GetCustomAttribute<SmartConfigAttribute>();

            if (smartConfig.SettingNameTarget == SettingNameTarget.Path) return true;

            if (smartConfig.SettingNameTarget == SettingNameTarget.None && configType.GetCustomAttribute<SettingNameAttribute>() != null) return true;

            return false;
        }
    }
}