using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reusable;
using Reusable.Data.Annotations;
using SmartConfig.Data;

namespace SmartConfig
{
    internal static class ConfigurationType
    {
        internal static List<SettingProperty> GetSettingProperties(Type configType)
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
    }
}