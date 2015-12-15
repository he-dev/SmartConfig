using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SmartConfig.Collections;
using SmartConfig.Converters;
using SmartConfig.Data;
using SmartConfig.IO;
using SmartConfig.Reflection;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Represents a configuration a provides methods for loading and updating settings.
    /// </summary>
    public static class Configuration
    {
        private static readonly IDictionary<Type, ConfigurationInfo> ConfigurationCache = new Dictionary<Type, ConfigurationInfo>();

        /// <summary>
        /// Gets the current converters and allows to add additional ones.
        /// </summary>
        public static ObjectConverterCollection Converters { get; } = new ObjectConverterCollection
        {
            new NumericConverter(),
            new BooleanConverter(),
            new StringConverter(),
            new EnumConverter(),
            new DateTimeConverter(),
            new ColorConverter(),
            new JsonConverter(),
            new XmlConverter(),
            new EmptyConverter(),
        };

        /// <summary>
        /// Loads settings for a a configuration from the specified data source.
        /// </summary>
        /// <param name="configType">SettingType that is marked with the <c>SmartCofnigAttribute</c> and specifies the configuration.</param>
        public static void LoadSettings(Type configType)
        {
            if (configType == null) { throw new ArgumentNullException(nameof(configType)); }

            var configuration = new ConfigurationInfo(configType);
            ConfigurationCache[configType] = configuration;

            SettingLoader.LoadSettings(configuration, Converters);
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
            var configuration = ConfigurationCache.Values.FirstOrDefault(x => x.SettingInfos.TryGetValue(property, out settingInfo));
            if (configuration == null)
            {
                throw new MemberNotFoundException { MemberName = property.Name };
            }

            SettingUpdater.UpdateSetting(settingInfo, value, Converters);
            settingInfo.Value = value;
        }
    }
}