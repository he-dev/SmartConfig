using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Reusable;
using Reusable.Converters;
using Reusable.DataAnnotations;
using Reusable.Extensions;
using Reusable.Validations;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;

namespace SmartConfig
{
    public class ConfigurationBuilder
    {
        private DataStore _dataStore;

        private TypeConverter _converter = CreateDefaultConverter();

        private Dictionary<string, object> _attributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        private Type _configType;

        internal ConfigurationBuilder() { }

        private static TypeConverter CreateDefaultConverter()
        {
            return TypeConverter.Empty.Add(new TypeConverter[]
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

        public ConfigurationBuilder From(DataStore dataStore)
        {
            _dataStore = dataStore.Validate(nameof(dataStore)).IsNotNull("You need to specify a data store.").Value;
            return this;
        }

        public ConfigurationBuilder Where(string name, object value)
        {
            name.Validate(nameof(name)).IsNotNullOrEmpty("You need to specify the namespace.");
            value.Validate(nameof(value)).IsNotNull("You need to specify the namespace value.");
            _attributes.ContainsKey(name).Validate().IsFalse($"Namespace '{name}' already exists.");
            _attributes.Add(name, value);
            return this;
        }

        public ConfigurationBuilder Where<T>(Expression<Func<T>> expression)
        {
            expression.Validate(nameof(expression)).IsNotNull();
            var memberExpression = (expression.Body as MemberExpression).Validate().IsNotNull($"The expression must be a {nameof(MemberExpression)}.").Value;
            return Where(memberExpression.Member.Name, expression.Compile()());
        }

        public Configuration Select(Type configType, string name = null, ConfigNameOption nameOption = ConfigNameOption.AsPath)
        {
            _configType = configType.Validate(nameof(configType)).IsNotNull("You need to specify the configuration type.").Value;

            configType.Validate(nameof(configType)).IsTrue(x => x.IsStatic(), $"Type {configType.Name} must be static.");

            configType.Validate(nameof(configType)).IsTrue(
                x => x.HasAttribute<SmartConfigAttribute>(),
                $"Type {configType.FullName} muss be decorated with the {nameof(SmartConfigAttribute)}.");

            var smartConfigAttribute = configType.GetCustomAttribute<SmartConfigAttribute>();

            // override attribute settings
            if (!string.IsNullOrEmpty(name))
            {
                smartConfigAttribute.Name = name;
                smartConfigAttribute.NameOption = nameOption;
            }

            if (smartConfigAttribute.NameOption == ConfigNameOption.AsNamespace)
            {
                _attributes.Add(nameof(SettingAttribute.Config), smartConfigAttribute.Name);
            }

            foreach (var typeConverterAttribute in configType.GetCustomAttributes<TypeConverterAttribute>())
            {
                _converter = _converter.Add(typeConverterAttribute.Type);
            }

            return ToConfiguration();
        }

        private Configuration ToConfiguration()
        {
            var configuration = new Configuration(_configType, _dataStore, _attributes, _converter);
            configuration.Reload();

            var temp = configuration;
            configuration = null;
            return temp;
        }
    }
}