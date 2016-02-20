using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SmartConfig.Collections;
using SmartConfig.Converters;
using SmartConfig.Data;
using SmartConfig.IO;

namespace SmartConfig
{
    /// <summary>
    /// Represents a configuration a provides methods for loading and updating settings.
    /// </summary>
    public class Configuration
    {
        private static readonly IDictionary<Type, Configuration> ConfigurationCache = new Dictionary<Type, Configuration>();

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
        };

        internal Configuration(Type configurationType)
        {
            Type = configurationType;
            Name = Type.GetCustomAttribute<SettingNameAttribute>()?.SettingName;
            Settings = new ReadOnlyCollection<Setting>(this.GetSettings().ToList());
        }

        internal Type Type { get; }

        internal string Name { get; }

        internal IReadOnlyCollection<Setting> Settings { get; }

        internal IDataStore DataStore { get; set; }

        internal List<SettingKey> AdditionalKeys { get; set; } = new List<SettingKey>();

        public static event EventHandler<ConfigurationReloadFailedEventArgs> ConfigurationReloadFailed = delegate { };

        public static ConfigurationBuilder Use(Type configurationType)
        {
            return new ConfigurationBuilder(configurationType);
        }

        internal static void Load(Configuration configuration)
        {
            SettingLoader.LoadSettings(configuration, Converters);
            ConfigurationCache[configuration.Type] = configuration;
        }

        public static void Reload(Type configurationType)
        {
            var configuration = (Configuration)null;
            if (!ConfigurationCache.TryGetValue(configurationType, out configuration))
            {
                throw new InvalidOperationException("Configuraiton not loaded.");
            }

            try
            {
                SettingLoader.LoadSettings(configuration, Converters);
            }
            catch (Exception ex)
            {
                ConfigurationReloadFailed(null, new ConfigurationReloadFailedEventArgs
                {
                    Exception = ex
                });
            }
        }

        public static void Save(Type configurationType)
        {
            var configuration = (Configuration)null;
            if (!ConfigurationCache.TryGetValue(configurationType, out configuration))
            {
                throw new InvalidOperationException("Configuraiton not loaded.");
            }

            foreach (var setting in configuration.Settings)
            {
                SettingUpdater.UpdateSetting(setting, setting.Value, Converters);
            }
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

            var settingInfo =
                ConfigurationCache.SelectMany(x => x.Value.Settings)
                .SingleOrDefault(si => si.Property == property);

            if (settingInfo == null)
            {
                throw new MemberNotFoundException { MemberName = property.Name };
            }

            SettingUpdater.UpdateSetting(settingInfo, value, Converters);
            settingInfo.Value = value;
        }
    }
}