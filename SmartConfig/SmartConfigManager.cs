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
        private static readonly Dictionary<Type, SmartConfigManager> SmartConfigManagers;

        public static readonly ObjectConverterCollection Converters;

        static SmartConfigManager()
        {
            SmartConfigManagers = new Dictionary<Type, SmartConfigManager>();

            Converters = new ObjectConverterCollection
            {
                new StringConverter(),
                new ValueTypeConverter(),
                new JsonConverter(),
                new XmlConverter(),
                new ColorConverter()
            };
        }

        /// <summary>
        /// Gets or sets the data source for the <c>SmartConfig</c>.
        /// </summary>
        private DataSourceBase DataSource { get; set; }

        /// <summary>
        /// Initializes a smart config.
        /// </summary>
        /// <typeparam name="TConfig">Type that is marked with the <c>SmartCofnigAttribute</c> and specifies the configuration.</typeparam>
        /// <param name="dataSource">Custom data source that provides data. If null <c>AppConfig</c> is used.</param>
        public static void Load<TConfig>(DataSourceBase dataSource)
        {
            #region SelfConfig initialization.

            var isSelfConfig = typeof(TConfig) == typeof(SelfConfig);
            if (!isSelfConfig)
            {
                var isSelfConfigInitialized = SmartConfigManagers.ContainsKey(typeof(SelfConfig));
                if (!isSelfConfigInitialized)
                {
                    Load<SelfConfig>();
                }
            }

            #endregion

            //if (dataSource == null)
            //{
            //    dataSource = new AppConfig();
            //}

            var smartConfig = new SmartConfigManager()
            {
                DataSource = dataSource
            };

            //InitializeConnectionString<TConfig>(dataSource);

            // Add new smart config.
            SmartConfigManagers[typeof(TConfig)] = smartConfig;
            Load<TConfig>();
        }

        public static void Update<TField>(Expression<Func<TField>> expression, TField value)
        {
            var memberInfo = GetMemberInfo(expression);

            // Update the field:
            ((FieldInfo)memberInfo).SetValue(null, value);

            //TField data = expression.Compile()();

            var converter = GetConverter((FieldInfo)memberInfo);
            var serializedData = converter.SerializeObject(value);

            var smartConfigType = GetSmartConfigType(memberInfo);
            var smartConfig = SmartConfigManagers[smartConfigType];

            var elementName = ConfigElementName.From(expression);
            smartConfig.DataSource.Update(new ConfigElement()
            {
                Environment = SelfConfig.AppSettings.Environment,
                Version = smartConfigType.Version().ToStringOrEmpty(),
                Name = elementName,
                Value = serializedData
            });
        }

        private static void Load<TConfig>()
        {
            var configName = (typeof(TConfig)).ConfigName();
            RecursiveLoad<TConfig>(typeof(TConfig), configName);
        }

        private static void RecursiveLoad<TConfig>(Type type, string typeName)
        {
            // Get and load fields:
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                Load<TConfig>(type, typeName, field);
            }

            var nestedTypes = type.GetNestedTypes();
            foreach (var nestedType in nestedTypes)
            {
                var fieldKey = ConfigElementName.From(typeName, nestedType.Name);
                RecursiveLoad<TConfig>(nestedType, fieldKey);
            }
        }

        private static void Load<TConfig>(Type type, string typeName, FieldInfo fieldInfo)
        {
            var dataSource = SmartConfigManagers[typeof(TConfig)].DataSource;
            var elementName = ConfigElementName.From(typeName, fieldInfo.Name);
            var configElements = dataSource.Select(elementName);

            configElements = FilterConfigElements<TConfig>(configElements);

            // Get the last element:
            var element = configElements.LastOrDefault();
            if (element == null)
            {
                // null is ok if the field is nullable:
                if (fieldInfo.FieldType.IsNullable())
                {
                    fieldInfo.SetValue(null, null);
                    return;
                }

                // null is not ok if the field is a value type:
                if (fieldInfo.FieldType.IsValueType)
                {
                    throw new ArgumentNullException(elementName, "Non nullable value type element does not allow null.");
                }

                // null is not ok if the attribute does not explicitly allow it:
                var allowNullData = fieldInfo.HasAttribute<AllowNullAttribute>();
                if (!allowNullData)
                {
                    throw new ArgumentNullException(elementName, "This field does not allow null data.");
                }

                // null is ok for reference types if allowed:
                fieldInfo.SetValue(null, null);
                return;
            }

            var converter = GetConverter(fieldInfo);
            var obj = converter.DeserializeObject(element.Value, fieldInfo.FieldType);
            fieldInfo.SetValue(null, obj);
        }

        private static ObjectConverterBase GetConverter(FieldInfo fieldInfo)
        {
            var type = fieldInfo.FieldType;

#if NET40
            var objectConverterAttr = fieldInfo.GetCustomAttributes(typeof(ObjectConverterAttribute), false).SingleOrDefault() as ObjectConverterAttribute;
#else
            var objectConverterAttr = fieldInfo.GetCustomAttribute<ObjectConverterAttribute>();
#endif
            if (objectConverterAttr != null)
            {
                type = objectConverterAttr.ObjectConverterType;
            }

            var objectConverter = Converters[type];
            if (objectConverter == null)
            {
                throw new ObjectConverterNotFoundException(type);
            }

            return objectConverter;
        }

        private static IEnumerable<ConfigElement> FilterConfigElements<TConfig>(IEnumerable<ConfigElement> configElements)
        {
            // Do not check the environment and version for SelfConfig because SelfConfig does not support it.
            var isSelfConfig = typeof(TConfig) == typeof(SelfConfig);
            if (isSelfConfig)
            {
                return configElements;
            }

            // Filter by Environment:
            if (!string.IsNullOrEmpty(SelfConfig.AppSettings.Environment))
            {
                configElements =
                    configElements
                    .Where(e => e.Environment.Equals(SelfConfig.AppSettings.Environment, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by version:
            var version = typeof(TConfig).Version();
            if (version != null)
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

        internal static Type GetSmartConfigType(MemberInfo memberInfo)
        {
            if (memberInfo.ReflectedType.HasAttribute<SmartConfigAttribute>())
            {
                return memberInfo.ReflectedType;
            }

            var type = memberInfo.DeclaringType;
            while (type != null)
            {
                if (type.HasAttribute<SmartConfigAttribute>())
                {
                    return type;
                }
                type = type.DeclaringType;
            }

            throw new SmartConfigAttributeNotFoundException("Neither the specified type nor any declaring type is marked with the SmartConfigAttribute.");
        }

        internal static MemberInfo GetMemberInfo<TField>(Expression<Func<TField>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }

            return memberExpression.Member;
        }

    }
}