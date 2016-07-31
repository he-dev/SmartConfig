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
using SmartUtilities.TypeConversion;
using SmartUtilities.TypeFramework;
using SmartUtilities.TypeFramework.Converters;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig
{
    public class Configuration
    {
        // Each loaded configuration is cached so that we don't have to run reflection mutliple times.
        private static readonly IDictionary<Type, Configuration> Cache = new Dictionary<Type, Configuration>();

        // The user must not create a configuration directly.
        internal Configuration() { }

        public static event EventHandler<ReloadFailedEventArgs> TryReloadFailed = delegate { };

        // Initializes the fluent interface.
        public static Builder Load => new Builder();

        // This might be useful in future.
        internal Type Type { get; private set; }

        // Stores info about each setting.
        internal IReadOnlyCollection<SettingInfo> Settings { get; private set; }

        // Stores info about namespaces.
        internal IReadOnlyDictionary<string, object> Namespaces { get; private set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        internal SettingReader SettingReader { get; private set; }

        internal SettingWriter SettingWriter { get; private set; }

        private int LoadInternal()
        {
            var affectedSettingCount = SettingReader.ReadSettings(Settings, Namespaces);
            Cache[Type] = this;
            return affectedSettingCount;
        }

        public int Save()
        {
            return SettingWriter.SaveSettings(Settings, Namespaces);
        }

        public static bool TryReload(Type configurationType)
        {
            try
            {
                var configuration = (Configuration)null;
                if (!Cache.TryGetValue(configurationType, out configuration))
                {
                    throw new InvalidOperationException($"Configuration {configurationType.Name} isn't loaded yet. To reload a configuration you need to load it first.");
                }

                configuration.SettingReader.ReadSettings(configuration.Settings, configuration.Namespaces);
                return true;
            }
            catch (Exception ex)
            {
                OnReloadFailed(configurationType, ex);
                return false;
            }
        }

        private static void OnReloadFailed(Type configurationType, Exception exception)
        {
            TryReloadFailed(null, new ReloadFailedEventArgs
            {
                ConfigurationType = configurationType,
                Exception = exception,
            });
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
                name.Validate(nameof(name)).IsNotNullOrEmpty(ctx => $"You need to specify the namespace.");
                value.Validate(nameof(value)).IsNotNull(ctx => $"You need to specify the namespace value.");
                _namespaces.ContainsKey(name).Validate().IsFalse(ctx => $"Namespace with this name '{name}' already exists.");
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
                    .IsNotNull(ctx => "You need to specify the configuration type.");

                type.Validate(nameof(type))
                    .OnFailure(ctx => new ClassNotStaticException(ctx.Argument))
                    .IsTrue(x => x.IsStatic());

                type.Validate(nameof(type))
                    .OnFailure(ctx => new SmartConfigAttributeNotFoundException(ctx.Argument))
                    .IsTrue(x => x.HasAttribute<SmartConfigAttribute>());

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

                _configuration.LoadInternal();

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