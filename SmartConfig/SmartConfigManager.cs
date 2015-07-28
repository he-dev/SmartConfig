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
        // Stores smart configs by type.
        private static readonly Dictionary<Type, DataSourceBase> ConfigDataSources;

        public static readonly ObjectConverterCollection Converters;

        static SmartConfigManager()
        {
            ConfigDataSources = new Dictionary<Type, DataSourceBase>();

            Converters = new ObjectConverterCollection
            {
                new StringConverter(),
                new ValueTypeConverter(),
                new EnumConverter(),
                new JsonConverter(),
                new XmlConverter(),
                new ColorConverter()
            };
        }

        /// <summary>
        /// Gets or sets the data source for the <c>SmartConfig</c>.
        /// </summary>
        private DataSourceBase DataSource { get; set; }

        #region Loading

        /// <summary>
        /// Initializes a smart config.
        /// </summary>
        /// <typeparam name="TConfig">Type that is marked with the <c>SmartCofnigAttribute</c> and specifies the configuration.</typeparam>
        /// <param name="configType"></param>
        /// <param name="dataSource">Custom data source that provides data. If null <c>AppConfig</c> is used.</param>
        public static void Load(Type configType, DataSourceBase dataSource)
        {
            if (!configType.IsStatic())
            {
                throw new InvalidOperationException("Config type must be a static class.");
            }

            if (!configType.HasAttribute<SmartConfigAttribute>())
            {
                throw new InvalidOperationException("Config type does not have the SmartConfigAttribute.");
            }

            #region SelfConfig initialization

            var selfConfiInitializationRequired = !ConfigDataSources.ContainsKey(typeof(SelfConfig)) && configType == typeof(SelfConfig);
            if (selfConfiInitializationRequired)
            {
                ConfigDataSources[typeof(SelfConfig)] = new AppConfig();
                LoadTree(typeof(SelfConfig), typeof(SelfConfig), Utilities.ConfigName(typeof(SelfConfig)));
            }

            #endregion

            // Add new smart config.
            ConfigDataSources[configType] = dataSource;
            LoadTree(configType, configType, Utilities.ConfigName(configType));
        }

        private static void LoadTree(Type configType, Type currentType, string currentClassName)
        {
            var fields = currentType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                LoadValue(configType, currentType, currentClassName, field);
            }

            var nestedTypes = currentType.GetNestedTypes();
            foreach (var nestedType in nestedTypes)
            {
                var nestedClassElementName = ConfigElementName.Combine(currentClassName, nestedType.Name);
                LoadTree(configType, nestedType, nestedClassElementName);
            }
        }

        private static void LoadValue(Type configType, Type currentType, string currentClassName, FieldInfo field)
        {
            var dataSource = ConfigDataSources[configType];
            var elementName = ConfigElementName.Combine(currentClassName, field.Name);
            var configElements = dataSource.Select(elementName);

            var canFilterConfigElements = !(dataSource is AppConfig);
            if (canFilterConfigElements)
            {
                configElements = FilterConfigElements(configType, configElements);
            }

            // Get the last element because it is the latest version.
            var element = configElements.LastOrDefault();
            if (element == null)
            {
                if (!field.IsOptional())
                {
                    throw new ConfigElementNotFounException(configType, elementName);
                }
            }
            else
            {
                var converter = GetConverter(field);
                var obj = converter.DeserializeObject(element.Value, field.FieldType, field.Contraints());
                if (obj == null && !field.IsNullable())
                {
                    throw new ValueNullException(configType, elementName);
                }
                field.SetValue(null, obj);
            }
        }

        #endregion

        public static void Update<TField>(Expression<Func<TField>> expression, TField value)
        {
            var memberInfo = Utilities.GetMemberInfo(expression);
            var configType = Utilities.GetSmartConfigType(memberInfo);
            var elementName = ConfigElementName.From(expression);
            var field = memberInfo as FieldInfo;

            if (!field.IsNullable() && value == null)
            {
                throw new ValueNullException(configType, elementName);
            }

            field.SetValue(null, value);

            //TField data = expression.Compile()();

            var converter = GetConverter(field);
            var serializedData = converter.SerializeObject(value, field.FieldType, field.GetCustomAttributes<ValueContraintAttribute>(true));

            var smartConfigType = Utilities.GetSmartConfigType(memberInfo);
            var dataSource = ConfigDataSources[smartConfigType];

            dataSource.Update(new ConfigElement()
            {
                Environment = SelfConfig.AppSettings.Environment,
                Version = smartConfigType.Version().ToStringOrEmpty(),
                Name = elementName,
                Value = serializedData
            });
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

        private static IEnumerable<ConfigElement> FilterConfigElements(Type configType, IEnumerable<ConfigElement> configElements)
        {
            var canFilterByEnvironment = !string.IsNullOrEmpty(SelfConfig.AppSettings.Environment);
            if (canFilterByEnvironment)
            {
                configElements =
                    configElements
                    .Where(e => e.Environment.Equals(SelfConfig.AppSettings.Environment, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by version:
            var version = configType.Version();
            var canFilterByVersion = version != null;
            if (canFilterByVersion)
            {
                configElements =
                    configElements
                    // Get versions that are less or equal current:
                    .Where(e => SemanticVersion.Parse(e.Version) <= version)
                    // Sort by version:
                    .OrderBy(e => SemanticVersion.Parse(e.Version));
            }
            return configElements;
        }
    }
}