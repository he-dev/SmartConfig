using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartConfig.IO;
using SmartUtilities;
using SmartUtilities.TypeFramework;
using SmartUtilities.TypeFramework.Converters;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig
{
    public class Configuration
    {
        // cache for all loaded configurations
        private static readonly IDictionary<Type, Configuration> Cache = new Dictionary<Type, Configuration>();

        public static Builder Load => new Builder();

        internal Configuration() { }

        internal Type Type { get; private set; }

        internal IReadOnlyCollection<SettingInfo> Settings { get; private set; }

        internal SettingReader SettingReader { get; private set; }

        internal SettingWriter SettingWriter { get; private set; }

        public static event EventHandler<ReloadFailedEventArgs> ReloadFailed = delegate { };

        //public static Configuration Build(Action<Builder> buildConfiguration)
        //{
        //    buildConfiguration.Validate(nameof(buildConfiguration)).IsNotNull(); //(ctx => "You need to specify a data store.");
        //    var builder = new Builder();
        //    buildConfiguration(builder);
        //    return builder.ToConfiguration();
        //}

        private int _Load()
        {
            var affectedSettingCount = SettingReader.ReadSettings(Settings);
            Cache[Type] = this;
            return affectedSettingCount;
        }

        private int _Save()
        {
            return SettingWriter.SaveSettings(Settings);
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
                configuration.SettingReader.ReadSettings(configuration.Settings);
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

        public static int Save(Type configurationType)
        {
            var configuration = (Configuration)null;
            if (!Cache.TryGetValue(configurationType, out configuration))
            {
                throw new InvalidOperationException($"To save a configuration you need to load it first. Configuration {configurationType.Name} isn't loaded yet.");
            }

            return configuration._Save();
        }

        public class Builder
        {
            private Configuration _configuration = new Configuration();

            internal Builder() { }

            private static TypeConverter CreateDefaultConverter()
            {
                return TypeConverter.Default.Register(new TypeConverter[]
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
                new StringToColorConverter(),
                new StringToBooleanConverter(),
                new StringToDateTimeConverter(),
                //new StringToEnumConverter<>(), 

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
                });
            }

            public Builder From(IDataStore dataStore)
            {
                dataStore.Validate(nameof(dataStore)).IsNotNull(ctx => "You need to specify a data store.");

                _configuration.SettingReader = new SettingReader(dataStore, CreateDefaultConverter());
                _configuration.SettingWriter = new SettingWriter(dataStore, CreateDefaultConverter());

                return this;
            }

            public Builder Where(string name, object value)
            {
                name.Validate(nameof(name)).IsNotNullOrEmpty();
                value.Validate(nameof(value)).IsNotNull();
                //_configuration.DataStore.Validate().IsNotNull(ctx => "You need to first specify the data store by calling the 'From' method.");
                //DataStore.SettingNamespaces[name] = value;

                _configuration.SettingReader.Namespaces.Add(name, value);
                _configuration.SettingWriter.Namespaces.Add(name, value);

                return this;
            }

            public Configuration Select(Type type, Func<TypeConverter, TypeConverter> configureConverter = null)
            {
                type.Validate(nameof(type))
                    .IsNotNull(ctx => "You need to specify a configuration type.")
                    .IsTrue(x => x.IsStatic(), ctx => $"Type '{type.FullName}' must be a static class.")
                    .IsTrue(x => x.HasAttribute<SmartConfigAttribute>(), ctx => $"Type '{type.FullName}' must be decorated with the '{nameof(SmartConfigAttribute)}'");

                _configuration.Type = type;
                _configuration.Settings = SettingCollection.From(type);

                if (configureConverter != null)
                {
                    var converter = configureConverter(CreateDefaultConverter());
                    _configuration.SettingReader.Converter = converter;
                    _configuration.SettingWriter.Converter = converter;
                }

                _configuration._Load();

                return ToConfiguration();
            }

            private Configuration ToConfiguration()
            {
                var temp = _configuration;
                _configuration = null;
                return temp;
            }
        }
    }
}