using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SmartUtilities;
using SmartUtilities.ObjectConverters.DataAnnotations;

namespace SmartConfig.Collections
{
    internal class SettingCollection : ReadOnlyCollection<Setting>
    {
        private SettingCollection(IList<Setting> list) : base(list) { }

        internal static SettingCollection Create(Configuration configuration)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }

            var types = configuration.Type.NestedTypes().Where(type => !type.HasAttribute<IgnoreAttribute>());

            var settings =
                types.Select(type => type.GetProperties(BindingFlags.Public | BindingFlags.Static))
                .SelectMany(properties => properties)
                .Where(property => !property.HasAttribute<IgnoreAttribute>())
                .Select(property => Setting.Create(property, configuration))
                .ToList();

            return new SettingCollection(settings);
        }
    }
}
