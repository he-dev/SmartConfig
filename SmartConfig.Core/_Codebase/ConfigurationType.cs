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
            var types = configType.NestedTypes(type => !type.HasAttribute<IgnoreAttribute>());

            return
                types
                    .Select(type => type.GetProperties(BindingFlags.Public | BindingFlags.Static))
                    .SelectMany(properties => properties)
                    .Where(property => !property.HasAttribute<IgnoreAttribute>())
                    .Select(property => new SettingProperty(
                        property, 
                        new SettingPath(names: property.GetPath().Where(Conditional.IsNotNullOrEmpty))
                    ));
        }         
    }
}