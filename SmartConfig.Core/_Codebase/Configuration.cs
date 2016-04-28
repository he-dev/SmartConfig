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
        // cache for all loaded configurations
        private static readonly IDictionary<Type, Configuration> Cache = new Dictionary<Type, Configuration>();

        // converters supported by this configuraiton
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

        internal IDataStore DataStore { get; private set; }

        public static event EventHandler<ReloadFailedEventArgs> ReloadFailed = delegate { };

        // initializes configuration and allows to configure converters
        public static Configuration Load(Type configurationType, Action<ObjectConverterCollection> converters = null)
        {
            if (configurationType == null)
            {
                throw new ArgumentNullException(nameof(configurationType));
            }

            if (!configurationType.IsStatic())
            {
                throw new TypeNotStaticException
                {
                    Type = configurationType.Name,
                    HelpText = "Configuration type must be a static class."
                };
            }

            if (!configurationType.HasAttribute<SmartConfigAttribute>())
            {
                throw new SmartConfigAttributeNotFoundException
                {
                    ConfigurationType = configurationType.Name,
                    HelpText = $"Configuration class must be decorated with the {nameof(SmartConfigAttribute)}."
                };
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
                throw new CustomKeyNullException
                {
                    SettingType = dataStore.SettingType.Name,
                    CustomKey = "[" + string.Join(", ", missingKeys) + "]",
                    HelpText = "Custom keys must not be null or empty."
                };
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
                throw new InvalidOperationException($"To reload a configuration you need to load it first. Configuration {configurationType.Name} isn't loaded yet.");
            }

            try
            {
                SettingLoader.LoadSettings(configuration);
            }
            catch (Exception ex)
            {
                ReloadFailed(null, new ReloadFailedEventArgs
                {
                    Exception = ex,
                    Configuration = configurationType.Name
                });
            }
        }

        public static void Save(Type configurationType)
        {
            var configuration = (Configuration)null;
            if (!Cache.TryGetValue(configurationType, out configuration))
            {
                throw new InvalidOperationException($"To save a configuration you need to load it first. Configuration {configurationType.Name} isn't loaded yet.");
            }

            foreach (var setting in configuration.Settings)
            {
                SettingUpdater.UpdateSetting(setting);
            }
        }
    }
}