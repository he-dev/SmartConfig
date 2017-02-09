using System;
using System.Collections.Generic;
using System.Linq;
using Reusable;
using SmartConfig.Data;
using SmartConfig.Services;
using Reusable.Converters;
using Reusable.Drawing;

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
        public static ConfigurationBuilder Builder => new ConfigurationBuilder();

        internal Type Type { get; }

        public void Load()
        {
            try
            {
                _settingReader.Read();
                Cache[Type] = this;
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
            if (!Cache.TryGetValue(configurationType, out Configuration configuration))
            {
                throw new InvalidOperationException($"Configuration {configurationType.Name} isn't loaded yet. To reload a configuration you need to load it first.");
            }

            configuration.Load();
        }

        public static void Save(Type configurationType)
        {
            if (!Cache.TryGetValue(configurationType, out Configuration configuration))
            {
                throw new InvalidOperationException($"To save a configuration you need to load it first. Configuration {configurationType.Name} isn't loaded yet.");
            }

            configuration.Save();
        }
    }
}