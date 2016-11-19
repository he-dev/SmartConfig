using System;
using System.Collections.Generic;
using System.Linq;
using Reusable;
using SmartConfig.Data;
using SmartConfig.Services;

namespace SmartConfig
{
    public partial class Configuration
    {
        // Each loaded configuration is cached so that we don't have to run reflection mutliple times.
        private static readonly IDictionary<Type, Configuration> Cache = new Dictionary<Type, Configuration>();

        // The user must not create a configuration directly.
        internal Configuration(Type configType, DataStore dataStore, IReadOnlyDictionary<string, object> attributes, TypeConverter converter)
        {
            Type = configType;
            DataStore = dataStore;
            Attributes = attributes;
            Converter = converter;
        }

        // Initializes the fluent interface.
        public static ConfigurationBuilder Load => new ConfigurationBuilder();

        // This might be useful in future.
        internal Type Type { get; }

        internal DataStore DataStore { get; }

        // Stores info about each setting.
        internal IEnumerable<SettingProperty> SettingProperties { get; private set; }

        // Stores info about namespaces.
        internal IReadOnlyDictionary<string, object> Attributes { get; private set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        internal TypeConverter Converter { get; }

        internal int Reload()
        {
            SettingProperties = Type.GetSettingProperties();

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

        public int Save()
        {
            // SettingWriter.SaveSettings(SettingProperties, Attributes);


            return 0; 
        }

        public static bool Reload(Type configurationType, Action<Type, Exception> onException = null)
        {
            try
            {
                var configuration = (Configuration)null;
                if (!Cache.TryGetValue(configurationType, out configuration))
                {
                    throw new InvalidOperationException($"Configuration {configurationType.Name} isn't loaded yet. To reload a configuration you need to load it first.");
                }

                configuration.Reload();
                return true;
            }
            catch (Exception ex)
            {
                onException?.Invoke(configurationType, ex);
                return false;
            }
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