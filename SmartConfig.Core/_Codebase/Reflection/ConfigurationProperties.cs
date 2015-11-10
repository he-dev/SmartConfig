using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig.Reflection
{
    internal class ConfigurationProperties
    {
        internal ConfigurationProperties(Type configType)
        {
            if (configType == null) { throw new ArgumentNullException(nameof(configType)); }

            DataSource = new AppConfigSource();
            CustomKeys = new ReadOnlyCollection<SettingKey>(Enumerable.Empty<SettingKey>().ToList());

            var propertiesClass = configType.GetNestedType("Properties", BindingFlags.Public | BindingFlags.Static);
            if (propertiesClass == null)
            {
                return;
            }

            var dataSourceProperty = propertiesClass.GetProperty("DataSource", BindingFlags.Public | BindingFlags.Static); //, typeof(IDataSource));
            if (dataSourceProperty != null)
            {
                DataSource = dataSourceProperty.GetValue(null) as IDataSource;
                if (DataSource == null) { throw new ArgumentNullException($"{configType.Name}.Properties.DataSource"); }
            }

            var customKeysClass = propertiesClass.GetNestedType("CustomKeys", BindingFlags.Public | BindingFlags.Static);
            if (customKeysClass != null)
            {
                var customKeyProperties = customKeysClass.GetProperties(BindingFlags.Public | BindingFlags.Static);
                if (customKeyProperties.Any(ckp => ckp.PropertyType != typeof(object)))
                {
                    throw new ArgumentException(null, $"{configType.Name}.Properties.CustomKeys");
                }

                CustomKeys = new ReadOnlyCollection<SettingKey>(
                    customKeyProperties.Select(x => new SettingKey(x.Name, (string)x.GetValue(null))).ToList());
            }
        }

        public IDataSource DataSource { get; }

        public IReadOnlyCollection<SettingKey> CustomKeys { get; }
    }
}
