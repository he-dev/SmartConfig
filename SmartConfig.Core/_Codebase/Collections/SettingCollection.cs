using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;
using SmartUtilities;
using SmartUtilities.DataAnnotations;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig.Collections
{
    internal class SettingCollection : ReadOnlyCollection<SettingInfo>
    {
        private SettingCollection(IList<SettingInfo> list) : base(list) { }

        internal static SettingCollection From(Type configType)
        {
            var types = configType.NestedTypes(type => !type.HasAttribute<IgnoreAttribute>());

            var settings =
                types.Select(type => type.GetProperties(BindingFlags.Public | BindingFlags.Static))
                .SelectMany(properties => properties)
                .Where(property => !property.HasAttribute<IgnoreAttribute>())
                .Select(property => new SettingInfo(property, configType))
                .ToList();

            return new SettingCollection(settings);
        }        
    }
}
