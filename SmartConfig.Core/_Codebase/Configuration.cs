﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SmartConfig.Collections;
using SmartConfig.Converters;
using SmartConfig.Data;
using SmartConfig.Reflection;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Represents a configuration a provides methods for loading and updating settings.
    /// </summary>
    public static class Configuration
    {
        private static readonly IDictionary<Type, ConfigurationInfo> Configurations;
        private static readonly DataSourceCollection DataSources;

        /// <summary>
        /// Gets the current converters and allows to add additional ones.
        /// </summary>
        public static ObjectConverterCollection Converters { get; }

        static Configuration()
        {
            Configurations = new Dictionary<Type, ConfigurationInfo>();
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
            if (configType == null) { throw new ArgumentNullException(nameof(configType)); }
            if (dataSource == null) { throw new ArgumentNullException(nameof(dataSource)); }

            //Logger.LogTrace(() => $"Loading \"{configType.Name}\" from \"{dataSource.GetType().Name}\"...");

            DataSources[configType] = dataSource;

            //if (dataSource.SettingsInitializationEnabled)
            //{
            //    var settingsUpdater = new SettingsUpdater(new ConfigurationReflector(), Converters, DataSources);
            //    var settingsInitializer = new SettingsInitializer(new ConfigurationReflector(), settingsUpdater, DataSources);
            //    settingsInitializer.InitializeSettings(configType);
            //}

            var configuration = new ConfigurationInfo(configType);
            Configurations[configType] = configuration;

            var settingsLoader = new SettingsLoader(Converters, DataSources);
            settingsLoader.LoadSettings(configuration);
        }

        /// <summary>
        /// Updates a setting.
        /// </summary>
        /// <typeparam name="T">The type of the setting property.</typeparam>
        /// <param name="expression">Member expression of the setting to be updated.</param>
        /// <param name="value">Value to be set.</param>
        public static void UpdateSetting<T>(Expression<Func<T>> expression, T value)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ExpressionBodyNotMemberExpressionException { MemberFullName = expression.Body.Type.FullName };
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new MemberNotPropertyException { MemberName = memberExpression.Member.Name };
            }

            SettingInfo settingInfo = null;
            var configuration = Configurations.Values.FirstOrDefault(x => x.SettingInfos.TryGetValue(property, out settingInfo));
            if (configuration == null)
            {
                throw new MemberNotFoundException { MemberName = property.Name };
            }

            var settingsUdater = new SettingsUpdater(Converters, DataSources);
            settingsUdater.UpdateSetting(settingInfo, value);
            settingInfo.Value = value;
        }
    }
}