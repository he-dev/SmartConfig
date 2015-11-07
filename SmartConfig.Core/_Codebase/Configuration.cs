using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SmartConfig.Collections;
using SmartConfig.Converters;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Represents a configuration a provides methods for loading and updating settings.
    /// </summary>
    public static class Configuration
    {
        private static readonly DataSourceCollection DataSources;

        /// <summary>
        /// Gets the current converters and allows to add additional ones.
        /// </summary>
        public static ObjectConverterCollection Converters { get; }

        static Configuration()
        {
            DataSources = new DataSourceCollection();

            // initialize default converters
            Converters = new ObjectConverterCollection
            {
                new NumericConverter(),
                new BooleanConverter(),
                new StringConverter(),
                new EnumConverter(),
                new DateTimeConverter(),
                new ColorConverter(),
                new JsonConverter(),
                new XmlConverter(),
            };
        }

        /// <summary>
        /// Loads settings for a a configuration from the specified data source.
        /// </summary>
        /// <param name="configType">SettingType that is marked with the <c>SmartCofnigAttribute</c> and specifies the configuration.</param>
        /// <param name="dataSource">Data source that provides data.</param>
        public static void LoadSettings(Type configType, IDataSource dataSource)
        {
            #region check arguments

            if (configType == null)
            {
                throw new ArgumentNullException(nameof(configType));
            }
            if (dataSource == null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }

            if (!configType.IsStatic())
            {
                throw new ConfigTypeNotStaticException { ConfigTypeFullName = configType.FullName };
            }

            if (configType.GetCustomAttribute<SmartConfigAttribute>() == null)
            {
                throw new SmartConfigAttributeMissingException { ConfigTypeFullName = configType.FullName };
            }

            #endregion

            //Logger.LogTrace(() => $"Loading \"{configType.Name}\" from \"{dataSource.GetType().Name}\"...");

            DataSources[configType] = dataSource;

            //if (dataSource.SettingsInitializationEnabled)
            //{
            //    var settingsUpdater = new SettingsUpdater(new ConfigurationReflector(), Converters, DataSources);
            //    var settingsInitializer = new SettingsInitializer(new ConfigurationReflector(), settingsUpdater, DataSources);
            //    settingsInitializer.InitializeSettings(configType);
            //}

            var settingsLoader = new SettingsLoader(new ConfigurationReflector(), Converters, DataSources);
            settingsLoader.LoadSettings(configType);
        }

        /// <summary>
        /// Updates a setting.
        /// </summary>
        /// <typeparam name="T">The type of the setting property.</typeparam>
        /// <param name="memberExpression">Member expression of the setting to be updated.</param>
        /// <param name="value">Value to be set.</param>
        public static void UpdateSetting<T>(Expression<Func<T>> memberExpression, T value)
        {
            if (memberExpression == null)
            {
                throw new ArgumentNullException(nameof(memberExpression), "You need specify an exprestion for the setting you want to update.");
            }

            var settingInfo = SettingInfo.From(memberExpression);
            Debug.Assert(settingInfo != null);

            var settingsUdater = new SettingsUpdater(new ConfigurationReflector(), Converters, DataSources);
            settingsUdater.UpdateSetting(settingInfo, value);
            settingInfo.Value = value;
        }
    }
}