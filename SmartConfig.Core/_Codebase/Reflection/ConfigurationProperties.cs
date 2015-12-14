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

            InitializeProperties(configType);
        }

        public string Name { get; private set; }

        public IDataSource DataSource { get; private set; } = new AppConfigSource();

        public IReadOnlyCollection<SettingKey> CustomKeys { get; private set; } = new ReadOnlyCollection<SettingKey>(Enumerable.Empty<SettingKey>().ToList());

        private void InitializeProperties(Type configType)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            var propertiesClass = configType.GetNestedType("Properties", bindingFlags);
            if (propertiesClass == null)
            {
                return;
            }

            InitializeNameProperty(propertiesClass);
            InitializeDataSourceProperty(propertiesClass);
            InitializeCustomKeysProperty(propertiesClass);
        }

        private void InitializeNameProperty(Type propertiesClass)
        {
            // try get Name property
            var nameProperty = propertiesClass.GetProperty("Name");
            if (nameProperty == null)
            {
                return;
            }

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

            if (string.IsNullOrEmpty(Name))
            {
                throw new ArgumentNullException(nameof(Name));
            }
        }

        private void InitializeDataSourceProperty(Type propertiesClass)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            var dataSourceProperty = propertiesClass.GetProperty("DataSource", bindingFlags);
            if (dataSourceProperty == null)
            {
                return;
            }

            DataSource = dataSourceProperty.GetValue(null) as IDataSource;
            if (DataSource == null)
            {
                throw new ArgumentNullException($"{propertiesClass.DeclaringType.Name}.Properties.DataSource");
            }
        }

        private void InitializeCustomKeysProperty(Type propertiesClass)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            var customKeysClass = propertiesClass.GetNestedType("CustomKeys", bindingFlags);
            if (customKeysClass == null)
            {
                return;
            }

            var customKeyProperties = customKeysClass.GetProperties(bindingFlags);
            if (customKeyProperties.Any(ckp => ckp.PropertyType != typeof(object)))
            {
                throw new ArgumentException(null, $"{propertiesClass.DeclaringType.Name}.Properties.CustomKeys");
            }

            var settingKeys = customKeyProperties.Select(x => new SettingKey(x.Name, (string)x.GetValue(null))).ToList();
            CustomKeys = new ReadOnlyCollection<SettingKey>(settingKeys);
        }

    }
}
