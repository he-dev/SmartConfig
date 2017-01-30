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
        public static ConfigurationBuilder Load => new ConfigurationBuilder();

        public static TypeConverter DefaultConverter() => TypeConverter.Empty.Add(new TypeConverter[]
        {
            new StringToSByteConverter(),
            new StringToByteConverter(),
            new StringToCharConverter(),
            new StringToInt16Converter(),
            new StringToInt32Converter(),
            new StringToInt64Converter(),
            new StringToUInt16Converter(),
            new StringToUInt32Converter(),
            new StringToUInt64Converter(),
            new StringToSingleConverter(),
            new StringToDoubleConverter(),
            new StringToDecimalConverter(),
            new StringToColorConverter(new ColorParser[]
            {
                new NameColorParser(),
                new DecimalColorParser(), 
                new HexadecimalColorParser(), 
            }),
            new StringToBooleanConverter(),
            new StringToDateTimeConverter(),
            new StringToEnumConverter(),

            new SByteToStringConverter(),
            new ByteToStringConverter(),
            new CharToStringConverter(),
            new Int16ToStringConverter(),
            new Int32ToStringConverter(),
            new Int64ToStringConverter(),
            new UInt16ToStringConverter(),
            new UInt32ToStringConverter(),
            new UInt64ToStringConverter(),
            new SingleToStringConverter(),
            new DoubleToStringConverter(),
            new DecimalToStringConverter(),
            new ColorToStringConverter(),
            new BooleanToStringConverter(),
            new DateTimeToStringConverter(),
            new EnumToStringConverter(),

            new EnumerableToArrayConverter(),
            new EnumerableToListConverter(),
            new EnumerableToHashSetConverter(),
            new DictionaryToDictionaryConverter(),
        });

        internal Type Type { get; }

        public void Reload()
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

        public static void Reload(Type configurationType)
        {
            var configuration = default(Configuration);
            if (!Cache.TryGetValue(configurationType, out configuration))
            {
                throw new InvalidOperationException($"Configuration {configurationType.Name} isn't loaded yet. To reload a configuration you need to load it first.");
            }

            configuration.Reload();
        }

        public static void Save(Type configurationType)
        {
            var configuration = default(Configuration);
            if (!Cache.TryGetValue(configurationType, out configuration))
            {
                throw new InvalidOperationException($"To save a configuration you need to load it first. Configuration {configurationType.Name} isn't loaded yet.");
            }

            configuration.Save();
        }
    }
}