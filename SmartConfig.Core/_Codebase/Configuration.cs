using System;
using System.Collections.Generic;
using System.Linq;
using Reusable;
using SmartConfig.Data;
using SmartConfig.Services;
using Reusable.Converters;

namespace SmartConfig
{
    public class Configuration
    {
        // Each loaded configuration is cached so that we don't have to run reflection mutliple times.
        private static readonly IDictionary<Type, Configuration> Cache = new Dictionary<Type, Configuration>();

        // The user must not create a configuration directly.
        internal Configuration(Type configType, DataStore dataStore, IReadOnlyDictionary<string, object> attributes,
            TypeConverter converter)
        {
            Type = configType;
            DataStore = dataStore;
            Attributes = attributes;
            Converter = converter;
        }

        // Initializes the fluent interface.
        public static ConfigurationBuilder Load => new ConfigurationBuilder();

        internal Type Type { get; }

        internal DataStore DataStore { get; }

        internal IEnumerable<SettingProperty> SettingProperties { get; private set; }

        internal IReadOnlyDictionary<string, object> Attributes { get; }

        internal TypeConverter Converter { get; }

        internal int Reload()
        {
            SettingProperties = Type.GetSettingProperties();

            try
            {
                // Load all settings.
                var settings = SettingProperties.Select(x => new
                {
                    Key = x,
                    Value = DataStore.GetSettings(new Setting
                    {
                        Name = x.Path,
                        Attributes = Attributes
                    }).ToList()
                }).ToDictionary(x => x.Key, x => x.Value);

                var settingDeserializer = new SettingDeserializer(Converter);
                var deserializedSettings = settingDeserializer.DeserializeSettings(settings);

                // Commit settings.
                foreach (var item in deserializedSettings)
                {
                    var settingProperty = item.Key;
                    settingProperty.Value = item.Value;
                }

                Cache[Type] = this;
                return deserializedSettings.Count;
            }
            catch (Exception innerException)
            {
                throw new ConfigurationLoadException(Type, DataStore.GetType(), innerException);
            }
        }

        public int Save()
        {
            try
            {
                var settingSerializer = new SettingSerializer(Converter);
                var allSettings = settingSerializer.SerializeSettings(SettingProperties, DataStore.SupportedTypes);
                foreach (var item in allSettings)
                {
                    var settings = item.Value;
                    foreach (var setting in settings)
                    {
                        setting.Attributes = Attributes;
                    }
                    DataStore.SaveSettings(settings);
                }
                return allSettings.Count;
            }
            catch (Exception innerException)
            {
                throw new ConfigurationSaveException(Type, DataStore.GetType(), innerException);
            }
        }

        public static int Reload(Type configurationType)
        {
            var configuration = (Configuration)null;
            if (!Cache.TryGetValue(configurationType, out configuration))
            {
                throw new InvalidOperationException($"Configuration {configurationType.Name} isn't loaded yet. To reload a configuration you need to load it first.");
            }

            return configuration.Reload();
        }

        public static int Save(Type configurationType)
        {
            var configuration = (Configuration)null;
            if (!Cache.TryGetValue(configurationType, out configuration))
            {
                throw new InvalidOperationException($"To save a configuration you need to load it first. Configuration {configurationType.Name} isn't loaded yet.");
            }

            return configuration.Save();
        }
    }
}