using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Reusable;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Data.Annotations;
using SmartConfig.Services;
using Reusable.TypeConversion;

namespace SmartConfig
{
    // Builds a configuration.
    public class ConfigurationBuilder
    {
        private readonly Action<string> _log;

        private List<SettingProperty> _settingProperties;

        private DataStore _dataStore;

        private TypeConverter _converter = TypeConverterFactory.CreateDefaultConverter();

        private TagCollection _tags = new TagCollection();

        private Type _configType;

        internal ConfigurationBuilder(Action<string> log)
        {
            _log = log;
        }        

        public ConfigurationBuilder From(DataStore dataStore)
        {
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            return this;
        }

        public ConfigurationBuilder Where(string name, object value)
        {
            _tags.Add(
                name ?? throw new ArgumentNullException(nameof(name)), 
                value ?? throw new ArgumentNullException(nameof(value))
            );
            return this;
        }

        public ConfigurationBuilder Where<T>(Expression<Func<T>> expression)
        {
            var memberExpression = 
                ((expression ?? throw new ArgumentNullException(nameof(expression))).Body as MemberExpression) 
                ?? throw new ArgumentException(nameof(expression), $"The expression must be a {nameof(MemberExpression)}.");
            return Where(memberExpression.Member.Name, expression.Compile()());
        }

        public ConfigurationBuilder Where(IDictionary<string, object> tags)
        {
            _tags = new TagCollection(tags ?? throw new ArgumentNullException(nameof(tags)));
            return this;
        }

        public Configuration Select(Type configType, Func<TypeConverter, TypeConverter> configureConverter)
        {
            if (configType == null) throw new ArgumentNullException(nameof(configType));
            if (!configType.IsStatic()) throw new ArgumentException(nameof(configType), $"Config type \"{configType.FullName}\" must be static.");
            if (!configType.HasAttribute<SmartConfigAttribute>()) throw new ArgumentException(nameof(configType), $"Config type \"{configType.FullName}\" muss be decorated with the {nameof(SmartConfigAttribute)}.");
            if (configureConverter == null) throw new ArgumentNullException(nameof(configureConverter));

            _configType = configType;

            AddCustomConverters(configType);

            _converter = configureConverter(_converter);
            _settingProperties = ConfigurationType.GetSettingProperties(configType).ToList();

            return ToConfiguration().Load();
        }

        public Configuration Select(Type configType)
        {
            return Select(configType, x => x);
        }

        private void AddCustomConverters(MemberInfo configType)
        {
            foreach (var typeConverterAttribute in configType.GetCustomAttributes<TypeConverterAttribute>())
            {
                _converter = _converter.Add(typeConverterAttribute.Type);
            }
        }        

        private Configuration ToConfiguration()
        {
            return new Configuration(
                _configType, 
                new SettingReader(_dataStore, _settingProperties, _tags, _converter, _log),
                new SettingWriter(_dataStore, _settingProperties, _tags, _converter, _log)
            );
        }
    }
}