using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Reusable;
using Reusable.Fuse;
using Reusable.Converters;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Data.Annotations;
using SmartConfig.Services;

namespace SmartConfig
{
    // Builds a configuration.
    public class ConfigurationBuilder
    {
        private List<SettingProperty> _settingProperties;

        private DataStore _dataStore;

        private TypeConverter _converter = TypeConverterFactory.CreateDefaultConverter();

        private TagCollection _tags = new TagCollection();

        private Type _configType;

        internal ConfigurationBuilder() { }        

        public ConfigurationBuilder From(DataStore dataStore)
        {
            _dataStore = dataStore.Validate(nameof(dataStore)).IsNotNull("You need to specify a data store.").Value;
            return this;
        }

        public ConfigurationBuilder Where(string name, object value)
        {
            name.Validate(nameof(name)).IsNotNullOrEmpty("You need to specify the namespace.");
            value.Validate(nameof(value)).IsNotNull("You need to specify the namespace value.");
            _tags.Add(name, value);
            return this;
        }

        public ConfigurationBuilder Where<T>(Expression<Func<T>> expression)
        {
            expression.Validate(nameof(expression)).IsNotNull();
            var memberExpression = (expression.Body as MemberExpression).Validate().IsNotNull($"The expression must be a {nameof(MemberExpression)}.").Value;
            return Where(memberExpression.Member.Name, expression.Compile()());
        }

        public ConfigurationBuilder Where(IDictionary<string, object> tags)
        {
            tags.Validate(nameof(tags)).IsNotNull("You need to specify the tags.");
            _tags = new TagCollection(tags);
            return this;
        }

        public Configuration Select(Type configType, Func<TypeConverter, TypeConverter> configureConverter)
        {
            _configType = configType.Validate(nameof(configType))
                .IsNotNull($"You need to specify the \"{nameof(configType)}\".")
                .IsTrue(x => x.IsStatic(), $"Config type \"{configType.FullName}\" must be static.")
                .IsTrue(x => x.HasAttribute<SmartConfigAttribute>(), $"Config type \"{configType.FullName}\" muss be decorated with the {nameof(SmartConfigAttribute)}.")
                .Value;

            GetConverters(configType);

            _converter = configureConverter?.Invoke(_converter) ?? _converter;
            _settingProperties = ConfigurationType.GetSettingProperties(configType).ToList();

            return ToConfiguration();
        }

        public Configuration Select(Type configType)
        {
            return Select(configType, null);
        }

        private void GetConverters(MemberInfo configType)
        {
            foreach (var typeConverterAttribute in configType.GetCustomAttributes<TypeConverterAttribute>())
            {
                _converter = _converter.Add(typeConverterAttribute.Type);
            }
        }        

        private Configuration ToConfiguration()
        {
            var settingReader = new SettingReader(_dataStore, _settingProperties, _tags, _converter);
            var settingWriter = new SettingWriter(_dataStore, _settingProperties, _tags, _converter);
            var configuration = new Configuration(_configType, settingReader, settingWriter);
            configuration.Load();

            var temp = configuration;
            configuration = null;
            return temp;
        }
    }
}