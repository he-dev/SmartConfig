using System;
using System.Collections.Generic;
using SmartConfig.Services;

namespace SmartConfig
{
    // Provides configuration APIs.
    public class Configuration
    {
        // Each loaded configuration is cached so that we don't have to run reflection mutliple times.
        private static readonly IDictionary<Type, Configuration> Cache = new Dictionary<Type, Configuration>();

        private readonly SettingReader _settingReader;
        private readonly SettingWriter _settingWriter;

        // The user must not create a configuration directly.
        internal Configuration(Type configType, SettingReader settingReader, SettingWriter settingWriter)
        {
            _settingReader = settingReader;
            _settingWriter = settingWriter;
            Type = configType;
        }

        // Initializes the fluent interface.
        public static ConfigurationBuilder Builder() => Builder(message => { });

        public static ConfigurationBuilder Builder(Action<string> log) => new ConfigurationBuilder(log);

        internal Type Type { get; }

        public Configuration Load()
        {
            try
            {
                _settingReader.Read();
                Cache[Type] = this;
                return this;
            }
            catch (Exception innerException)
            {
                throw new ConfigurationException(Type, innerException);
            }
        }

        public void Save()
        {
            try
            {
                _settingWriter.Write();
            }
            catch (Exception innerException)
            {
                throw new ConfigurationException(Type, innerException);
            }
        }

        public static void Load(Type configurationType)
        {
            if (Cache.TryGetValue(configurationType, out Configuration configuration))
            {
                configuration.Load();
            }
            else
            {
                throw new InvalidOperationException($"Cannot load configuration '{configurationType.Name}'. Use the '{nameof(Builder)}' to initialize it first.");
            }
        }

        public static void Save(Type configurationType)
        {
            if (Cache.TryGetValue(configurationType, out Configuration configuration))
            {
                configuration.Save();
            }
            else
            {
                throw new InvalidOperationException($"Cannot save configuration '{configurationType.Name}'. Use the '{nameof(Builder)}' to initialize it first.");
            }
        }
    }
}