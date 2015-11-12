using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig.Reflection
{
    internal class ConfigProperties
    {
        internal ConfigProperties(Type configType)
        {
            if (configType == null) { throw new ArgumentNullException(nameof(configType)); }

            DataSource = new AppConfigSource();
            CustomKeys = new ReadOnlyCollection<SettingKey>(Enumerable.Empty<SettingKey>().ToList());

            var bindingFlags = BindingFlags.Public | BindingFlags.Static;

            var propertiesClass = configType.GetNestedType("Properties", bindingFlags);
            if (propertiesClass == null)
            {
                return;
            }

            var nameProperty = propertiesClass.GetProperty("Name");
            if (nameProperty.PropertyType != typeof(string))
            {
                throw new InvalidMemberTypeException
                {
                    MemberName = nameProperty.Name,
                    DeclaringTypeFullName = nameProperty.DeclaringType.FullName,
                    MemberTypeFullName = nameProperty.PropertyType.FullName,
                    ExpectedTypeFullName = typeof(string).FullName
                };
            }
            Name = (string)nameProperty.GetValue(null);
            if (string.IsNullOrEmpty(Name)) { throw new ArgumentNullException(nameof(Name)); }

            var dataSourceProperty = propertiesClass.GetProperty("DataSource", bindingFlags); //, typeof(IDataSource));
            if (dataSourceProperty != null)
            {
                DataSource = dataSourceProperty.GetValue(null) as IDataSource;
                if (DataSource == null) { throw new ArgumentNullException($"{configType.Name}.Properties.DataSource"); }
            }

            var customKeysClass = propertiesClass.GetNestedType("CustomKeys", bindingFlags);
            if (customKeysClass != null)
            {
                var customKeyProperties = customKeysClass.GetProperties(bindingFlags);
                if (customKeyProperties.Any(ckp => ckp.PropertyType != typeof(object)))
                {
                    throw new ArgumentException(null, $"{configType.Name}.Properties.CustomKeys");
                }

                CustomKeys = new ReadOnlyCollection<SettingKey>(
                    customKeyProperties.Select(x => new SettingKey(x.Name, (string)x.GetValue(null))).ToList());
            }
        }

        public string Name { get; }

        public IDataSource DataSource { get; }

        public IReadOnlyCollection<SettingKey> CustomKeys { get; }
    }
}
