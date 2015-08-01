using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartConfig.Collections;
using SmartConfig.Converters;
using SmartConfig.Data;

namespace SmartConfig
{
    /// <summary>
    /// Provides all the <c>SmartConfig</c> functionality.
    /// </summary>
    public class SmartConfigManager
    {
        private static readonly Dictionary<Type, IDataSource> DataSources;

        public static readonly ObjectConverterCollection Converters;

        static SmartConfigManager()
        {
            DataSources = new Dictionary<Type, IDataSource>();

            Converters = new ObjectConverterCollection
            {
                new ColorConverter(),
                new DateTimeConverter(),
                new EnumConverter(),
                new JsonConverter(),
                new StringConverter(),
                new ValueTypeConverter(),
                new XmlConverter(),
            };
        }

        #region Loading

        /// <summary>
        /// Initializes a smart config.
        /// </summary>
        /// <typeparam name="TConfig">Type that is marked with the <c>SmartCofnigAttribute</c> and specifies the configuration.</typeparam>
        /// <param name="configType"></param>
        /// <param name="dataSource">Custom data source that provides data. If null <c>AppConfig</c> is used.</param>
        public static void Load(Type configType, IDataSource dataSource)
        {
            if (!configType.IsStatic())
            {
                throw new InvalidOperationException("Config type must be a static class.");
            }

            if (!configType.HasAttribute<SmartConfigAttribute>())
            {
                throw new SmartConfigTypeNotFoundException("Config type must have the SmartConfigAttribute.");
            }

            DataSources[configType] = dataSource;

            var fields = GetConfigFields(configType);
            foreach (var field in fields)
            {
                LoadValue(field);
            }
        }

        private static IEnumerable<FieldInfo> GetConfigFields(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.GetCustomAttribute<IgnoreAttribute>() == null);
            foreach (var field in fields)
            {
                yield return field;
            }

            var nestedTypes =
                type
                .GetNestedTypes()
                .Where(t => t.GetCustomAttribute<IgnoreAttribute>() == null)
                .SelectMany(GetConfigFields);

            foreach (var field in nestedTypes)
            {
                yield return field;
            }
        }

        private static void LoadValue(FieldInfo field)
        {
            var configFieldInfo = Utilities.GetConfigFieldInfo(field);
            var value =
                DataSources[configFieldInfo.ConfigType]
                .Select(new Dictionary<string, string>(configFieldInfo.Keys) { { "Name", configFieldInfo.ElementName } });

            if (string.IsNullOrEmpty(value))
            {
                if (field.IsOptional())
                {
                    return;
                }
                throw new OptionalException(configFieldInfo.ConfigType, configFieldInfo.ElementName);
            }

            try
            {
                var converter = GetConverter(field);
                var obj = converter.DeserializeObject(value, field.FieldType, field.Contraints());
                field.SetValue(null, obj);
            }
            catch (Exception ex)
            {
                throw new ObjectConverterException(configFieldInfo.ConfigType, configFieldInfo.ElementName, ex);
            }
        }

        #endregion

        public static void Update<TField>(Expression<Func<TField>> expression, TField value)
        {
            var memberInfo = Utilities.GetMemberInfo(expression);
            var configType = Utilities.GetConfigType(memberInfo);
            var elementName = ConfigElementName.From(expression);
            var field = memberInfo as FieldInfo;

            field.SetValue(null, value);

            //TField data = expression.Compile()();

            if (value == null && !field.IsNullable())
            {
                throw new NullableException(configType, elementName);
            }

            try
            {
                var converter = GetConverter(field);
                var valueSerialized = converter.SerializeObject(value, field.FieldType, field.GetCustomAttributes<ValueConstraintAttribute>(false));

                var configFieldInfo = Utilities.GetConfigFieldInfo(memberInfo);
                DataSources[configFieldInfo.ConfigType]
                    .Update(
                        new Dictionary<string, string>(configFieldInfo.Keys) { { "Name", configFieldInfo.ElementName } },
                        valueSerialized);
            }
            catch (Exception ex)
            {
                throw new ObjectConverterException(configType, elementName, ex);
            }
        }

        private static ObjectConverterBase GetConverter(FieldInfo fieldInfo)
        {
            var type = fieldInfo.FieldType;

            if (type.BaseType == typeof(Enum))
            {
                type = typeof(Enum);
            }
            else
            {
                var objectConverterAttr = fieldInfo.GetCustomAttribute<ObjectConverterAttribute>(true);
                if (objectConverterAttr != null)
                {
                    type = objectConverterAttr.Type;
                }
            }

            var objectConverter = Converters[type];
            if (objectConverter == null)
            {
                throw new ObjectConverterNotFoundException(type);
            }

            return objectConverter;
        }

    }
}