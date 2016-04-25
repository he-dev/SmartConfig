using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartConfig.IO;
using SmartUtilities;
using SmartUtilities.Collections;
using SmartUtilities.ObjectConverters;

namespace SmartConfig
{
    /// <summary>
    /// Represents a configuration a provides methods for loading and updating settings.
    /// </summary>
    public class Configuration
    {
        private static readonly IDictionary<Type, Configuration> Cache = new Dictionary<Type, Configuration>();

        /// <summary>
        /// Gets the current converters and allows to add additional ones.
        /// </summary>
        //public static ObjectConverterCollection Converters { get; } = new ObjectConverterCollection
        //{
        //    new NumericConverter(),
        //    new BooleanConverter(),
        //    new StringConverter(),
        //    new EnumConverter(),
        //    new DateTimeConverter(),
        //    new ColorConverter(),
        //    //new JsonConverter(),
        //    new XmlConverter(),
        //};

        public ObjectConverterCollection Converters { get; } = new ObjectConverterCollection
        {
            new NumericConverter(),
            new BooleanConverter(),
            new StringConverter(),
            new EnumConverter(),
            new DateTimeConverter(),
            new ColorConverter(),
            new XmlConverter(),
        };

        private Configuration(Type configurationType)
        {
            Type = configurationType;
            Settings = SettingCollection.Create(this);
        }

        internal Type Type { get; }

        internal string Name => Type.GetCustomAttribute<CustomNameAttribute>()?.Name;

        internal IReadOnlyCollection<Setting> Settings { get; }

        internal IDataStore DataStore { get; set; }

        // internal IDictionary<string, object> CustomKeys { get; } = new Dictionary<string, object>();

        public static event EventHandler<ReloadFailedEventArgs> ReloadFailed = delegate { };

        public static Configuration Load(Type configurationType, Action<ObjectConverterCollection> converters = null)
        {
            if (configurationType == null)
            {
                throw new ArgumentNullException(nameof(configurationType));
            }

            if (!configurationType.IsStatic())
            {
                throw new TypeNotStaticException { Type = configurationType.Name };
            }

            if (!configurationType.HasAttribute<SmartConfigAttribute>())
            {
                throw new SmartConfigAttributeNotFoundException { ConfigurationType = configurationType.Name };
            }

            var configuration = new Configuration(configurationType);
            converters?.Invoke(configuration.Converters);
            return configuration;
        }

        public void From(IDataStore dataStore, Action<IDataStore> configureDataStore = null)
        {
            if (dataStore == null) { throw new ArgumentNullException(nameof(dataStore)); }

            configureDataStore?.Invoke(dataStore);

            // check if all custom keys have values
            var missingKeys = dataStore.CustomKeyFilters.Where(x => dataStore.CustomKeyValues[x.Key] == null).Select(x => x.Key).ToList();
            if (missingKeys.Any())
            {
                throw new ArgumentException($"You need to set the {dataStore.SettingType}'s following custom keys: {string.Join(", ", missingKeys)}");
            }


            DataStore = dataStore;

            SettingLoader.LoadSettings(this);
            Cache[Type] = this;
        }

        public static void Reload(Type configurationType)
        {
            var configuration = (Configuration)null;
            if (!Cache.TryGetValue(configurationType, out configuration))
            {
                throw new InvalidOperationException("Configuraiton not loaded.");
            }

            try
            {
                SettingLoader.LoadSettings(configuration);
            }
            catch (Exception ex)
            {
                ReloadFailed(null, new ReloadFailedEventArgs
                {
                    Exception = ex
                });
            }
        }

        public static void Save(Type configurationType)
        {
            var configuration = (Configuration)null;
            if (!Cache.TryGetValue(configurationType, out configuration))
            {
                throw new InvalidOperationException("Configuraiton not loaded.");
            }

            foreach (var setting in configuration.Settings)
            {
                SettingUpdater.UpdateSetting(setting);
            }
        }


        //    /// <summary>
        //    /// Updates a setting.
        //    /// </summary>
        //    /// <typeparam name="T">The type of the setting property.</typeparam>
        //    /// <param name="expression">Member expression of the setting to be updated.</param>
        //    /// <param name="value">Value to be set.</param>
        //    public static void UpdateSetting<T>(Expression<Func<T>> expression, T value)
        //    {
        //        if (expression == null)
        //        {
        //            throw new ArgumentNullException(nameof(expression));
        //        }

        //        var memberExpression = expression.Body as MemberExpression;
        //        if (memberExpression == null)
        //        {
        //            throw new ExpressionBodyNotMemberExpressionException { MemberFullName = expression.Body.Type.FullName };
        //        }

        //        var property = memberExpression.Member as PropertyInfo;
        //        if (property == null)
        //        {
        //            throw new MemberNotPropertyException { MemberName = memberExpression.Member.Name };
        //        }

        //        var settingInfo =
        //            Cache.SelectMany(x => x.Value.Settings)
        //            .SingleOrDefault(si => si.Property == property);

        //        if (settingInfo == null)
        //        {
        //            throw new MemberNotFoundException { MemberName = property.Name };
        //        }

        //        SettingUpdater.UpdateSetting(settingInfo, value, Converters);
        //        settingInfo.Value = value;
        //    }
    }
}