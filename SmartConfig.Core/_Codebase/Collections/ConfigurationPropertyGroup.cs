using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig.Collections
{
    // Loads and validate configuration properties that can be customized by the user.
    internal class ConfigurationPropertyGroup
    {
        internal ConfigurationPropertyGroup(Type configType)
        {
            if (configType == null) { throw new ArgumentNullException(nameof(configType)); }

            InitializeProperties(configType);
        }

        public IDataSource DataSource { get; private set; } = new AppConfigSource();

        //public IReadOnlyCollection<SettingKey> CustomKeys { get; private set; } = 
        //    new ReadOnlyCollection<SettingKey>(Enumerable.Empty<SettingKey>().ToList());

        public IEnumerable<SettingKey> CustomKeys { get; private set; } = Enumerable.Empty<SettingKey>();

        private void InitializeProperties(Type configType)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            var propertyGroup = configType.GetNestedTypes(bindingFlags).FirstOrDefault(t => t.HasAttribute<SmartConfigPropertiesAttribute>());
            if (propertyGroup == null)
            {
                return;
            }

            //InitializeNameProperty(propertyGroup);
            InitializeDataSourceProperty(propertyGroup);
            InitializeCustomKeysProperty(propertyGroup);
        }

        //private void InitializeNameProperty(Type propertyGroup)
        //{
        //    // try get Name property
        //    var nameProperty = propertyGroup.GetProperty("Name");
        //    if (nameProperty == null)
        //    {
        //        return;
        //    }

        //    if (nameProperty.PropertyType != typeof(string))
        //    {
        //        throw new InvalidMemberTypeException
        //        {
        //            MemberName = nameProperty.Name,
        //            DeclaringTypeFullName = nameProperty.DeclaringType.FullName,
        //            MemberTypeFullName = nameProperty.PropertyType.FullName,
        //            ExpectedTypeFullName = typeof(string).FullName
        //        };
        //    }

        //    Name = (string)nameProperty.GetValue(null);

        //    if (string.IsNullOrEmpty(Name))
        //    {
        //        throw new ArgumentNullException(nameof(Name));
        //    }
        //}

        private void InitializeDataSourceProperty(Type propertyGroup)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            var dataSourceProperty = 
                propertyGroup.GetProperties(bindingFlags)
                .FirstOrDefault(p => typeof(IDataSource).IsAssignableFrom(p.PropertyType));

            if (dataSourceProperty == null)
            {
                return;
            }

            DataSource = dataSourceProperty.GetValue(null) as IDataSource;
            if (DataSource == null)
            {
                throw new ArgumentNullException(dataSourceProperty.Name, "Data source must not be null.");
            }
        }

        private void InitializeCustomKeysProperty(Type propertyGroup)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            var customKeysProperty = 
                propertyGroup.GetProperties(bindingFlags)
                .FirstOrDefault(p => typeof(IEnumerable<SettingKey>).IsAssignableFrom(p.PropertyType));

            if (customKeysProperty == null)
            {
                return;
            }

            CustomKeys = customKeysProperty.GetValue(null) as IEnumerable<SettingKey>;

            if (CustomKeys == null)
            {
                throw new ArgumentNullException(customKeysProperty.Name, "Custom keys property must not be null.");
            }
        }
    }
}
