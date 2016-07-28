using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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

        internal IReadOnlyDictionary<string, object> Namespaces { get; private set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        internal SettingReader SettingReader { get; private set; }

        internal SettingWriter SettingWriter { get; private set; }

        public static event EventHandler<ReloadFailedEventArgs> ReloadFailed = delegate { };      

        private int _Load()
        {
            var affectedSettingCount = SettingReader.ReadSettings(Settings, Namespaces);
            Cache[Type] = this;
            return affectedSettingCount;
        }

        private int _Save()
        {
            return SettingWriter.SaveSettings(Settings, Namespaces);
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
                configuration.SettingReader.ReadSettings(configuration.Settings, configuration.Namespaces);
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

            private IDataStore _dataStore;

            private TypeConverter _converter = CreateDefaultConverter();

            private Dictionary<string, object> _namespaces = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

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

                    new EnumerableObjectToArrayObjectConverter(),
                    new EnumerableObjectToListObjectConverter(),
                    new EnumerableObjectToHashSetObjectConverter(),
                    new DictionaryObjectObjectToDictionaryObjectObjectConverter(),

                    new EnumerableObjectToArrayStringConverter(),
                    new EnumerableObjectToListStringConverter(),
                    new EnumerableObjectToHashSetStringConverter(),
                    new DictionaryObjectObjectToDictionaryStringStringConverter(),
                });
            }

            public Builder From(IDataStore dataStore)
            {
                _dataStore = dataStore.Validate(nameof(dataStore)).IsNotNull(ctx => "You need to specify a data store.").Argument;                
                return this;
            }

            public Builder Where(string name, object value)
            {
                name.Validate(nameof(name)).IsNotNullOrEmpty();
                value.Validate(nameof(value)).IsNotNull();
                //_configuration.DataStore.Validate().IsNotNull(ctx => "You need to first specify the data store by calling the 'From' method.");
                //DataStore.SettingNamespaces[name] = value;

                _namespaces.Add(name, value);

                return this;
            }

            public Builder Register<TConverter>() where TConverter : TypeConverter, new()
            {
                _converter = _converter.Register<TConverter>();
                return this;
            }

            public Configuration Select(Type type, string name = null, ConfigNameOption nameOption = ConfigNameOption.AsPath)
            {
                type.Validate(nameof(type))
                    .IsNotNull(ctx => "You need to specify a configuration type.")
                    .IsTrue(x => x.IsStatic(), ctx => $"Type '{type.FullName}' must be a static class.")
                    .IsTrue(x => x.HasAttribute<SmartConfigAttribute>(), ctx => $"Type '{type.FullName}' must be decorated with the '{nameof(SmartConfigAttribute)}'");

                var smartConfigAttribute = type.GetCustomAttribute<SmartConfigAttribute>();

                // override attribute settings
                if (!string.IsNullOrEmpty(name))
                {
                    smartConfigAttribute.Name = name;
                    smartConfigAttribute.NameOption = nameOption;
                }

                if (smartConfigAttribute.NameOption == ConfigNameOption.AsNamespace)
                {
                    _namespaces.Add(nameof(Setting.Config), smartConfigAttribute.Name);
                }

                _configuration.Type = type;
                _configuration.Settings = SettingCollection.From(type);
                _configuration.Namespaces = _namespaces;
                _configuration.SettingReader = new SettingReader(_dataStore, _converter);
                _configuration.SettingWriter = new SettingWriter(_dataStore, _converter);

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