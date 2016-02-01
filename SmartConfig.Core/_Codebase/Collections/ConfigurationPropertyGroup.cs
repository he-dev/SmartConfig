using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;
using SmartConfig.Reflection;

namespace SmartConfig.Collections
{
    // Loads and validates configuration properties that can be customized by the user.
    internal class ConfigurationPropertyGroup
    {
        private Type _propertyGroupType;

        private readonly IDataSource _defaultDataSource = new AppConfigSource();
        private readonly IEnumerable<SettingKey> _defaultCustomKeys = Enumerable.Empty<SettingKey>();

        private Func<IDataSource> _dataSourceGetter;
        private Func<IEnumerable<SettingKey>> _customKeysGetter;

        internal ConfigurationPropertyGroup(Type configType)
        {
            if (configType == null) { throw new ArgumentNullException(nameof(configType)); }

            InitializePropertyGroupType(configType);
        }

        public IDataSource DataSource
        {
            get
            {
                // we use a getter delegate so that we can change the data source at runtime
                if (_dataSourceGetter == null)
                {
                    var dataSourceProperty =
                        _propertyGroupType?.GetProperties(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(p => typeof(IDataSource).IsAssignableFrom(p.PropertyType));

                    // there is no custom data source
                    if (dataSourceProperty == null)
                    {
                        // make the getter delegate return the default data source
                        _dataSourceGetter = () => _defaultDataSource;
                    }
                    else
                    {
                        // create the getter delegate for the custom property
                        _dataSourceGetter = (Func<IDataSource>)Delegate.CreateDelegate(
                            typeof(Func<IDataSource>),
                            null,
                            dataSourceProperty.GetGetMethod());
                    }
                }

                // get the current data source
                var dataSource = _dataSourceGetter();

                if (dataSource == null)
                {
                    throw new NullReferenceException($"Data source must not be null. Check the \"{_propertyGroupType.DeclaringType.Name}\" configuration.");
                }
                return dataSource;
            }
        }

        public IEnumerable<SettingKey> CustomKeys
        {
            get
            {
                if (_customKeysGetter == null)
                {
                    var customKeysProperty =
                        _propertyGroupType?.GetProperties(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(p => typeof(IEnumerable<SettingKey>).IsAssignableFrom(p.PropertyType));

                    if (customKeysProperty == null)
                    {
                        _customKeysGetter = () => _defaultCustomKeys;
                    }
                    else
                    {
                        _customKeysGetter = (Func<IEnumerable<SettingKey>>)Delegate.CreateDelegate(
                            typeof(Func<IEnumerable<SettingKey>>),
                            null,
                            customKeysProperty.GetGetMethod());
                    }
                }


                var customKeys = _customKeysGetter();

                if (customKeys == null)
                {
                    throw new NullReferenceException($"Custom keys must not be null. Check the \"{_propertyGroupType.DeclaringType.Name}\" configuration.");
                }

                return customKeys;
            }
        }

        private void InitializePropertyGroupType(Type configType)
        {
            _propertyGroupType =
                configType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(t => t.HasAttribute<SmartConfigPropertiesAttribute>());
        }
    }
}
