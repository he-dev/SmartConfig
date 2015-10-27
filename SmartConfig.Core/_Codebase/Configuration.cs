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
        private static readonly DataSourceCollection DataSources = new DataSourceCollection();

        /// <summary>
        /// Gets the current converters and allows to add additional ones.
        /// </summary>
        public static ObjectConverterCollection Converters { get; }

        static Configuration()
        {
            // initialize default converters
            Converters = new ObjectConverterCollection
            {
                new ColorConverter(),
                new DateTimeConverter(),
                new EnumConverter(),
                new JsonConverter(),
                new StringConverter(),
                new ValueTypeConverter(),
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
                throw new ArgumentNullException(nameof(configType), "You need to specify a config type.");
            }
            if (dataSource == null)
            {
                throw new ArgumentNullException(nameof(dataSource), "You need to specify a data source.");
            }

            if (!configType.IsStatic())
            {
                throw new InvalidOperationException("'configType' must be a static class.");
            }

            if (configType.GetCustomAttribute<SmartConfigAttribute>() == null)
            {
                throw new InvalidOperationException("'configType' must be marked with the SmartConfigAttribute.");
            }

            #endregion

            //Logger.LogTrace(() => $"Loading \"{configType.Name}\" from \"{dataSource.GetType().Name}\"...");

            DataSources[configType] = dataSource;

            //if (dataSource.SettingsInitializationEnabled)
            //{
            //    var settingsUpdater = new SettingsUpdater(new ConfigReflector(), Converters, DataSources);
            //    var settingsInitializer = new SettingsInitializer(new ConfigReflector(), settingsUpdater, DataSources);
            //    settingsInitializer.InitializeSettings(configType);
            //}

            var settingsLoader = new SettingsLoader(new ConfigReflector(), Converters, DataSources);
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

            var settingsUdater = new SettingsUpdater(new ConfigReflector(), Converters, DataSources);
            settingsUdater.UpdateSetting(settingInfo, value);
            settingInfo.Value = value;
        }
    }
}