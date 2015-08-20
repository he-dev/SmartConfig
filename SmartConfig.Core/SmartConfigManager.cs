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
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// This class takes care of loading and updating config values.
    /// </summary>
    public static class SmartConfigManager
    {
        private static readonly Dictionary<Type, IDataSource> DataSources;

        /// <summary>
        /// Gets the converters collection that holds all the default converters and allows to add additional ones.
        /// </summary>
        public static ObjectConverterCollection Converters { get; private set; }

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
        /// Loads a configuration from the specified data source.
        /// </summary>
        /// <param name="configType">Type that is marked with the <c>SmartCofnigAttribute</c> and specifies the configuration.</param>
        /// <param name="dataSource">Data source that provides data.</param>
        public static void Load(Type configType, IDataSource dataSource)
        {
            if (configType == null) throw new ArgumentNullException("configType", "You need to specify a config type.");
            if (dataSource == null) throw new ArgumentNullException("dataSource", "You need to specify a data source.");
            if (!configType.IsStatic()) throw new InvalidOperationException("'configType' must be a static class.");

            var smartConfigAttribute = configType.GetCustomAttribute<SmartConfigAttribute>();

            if (smartConfigAttribute == null) throw new SmartConfigTypeNotFoundException() { ConfigType = configType };

            Logger.LogAction(() => "Loading [$configTypeName] from [$dataSourceName]...".FormatWith(new
            {
                configTypeName = configType.Name,
                dataSourceName = dataSource.GetType().Name
            }, true));

            DataSources[configType] = dataSource;           

            var settingInfos = Utilities.GetSettingInfos(configType).ToList();

            if (dataSource.CanInitializeSettings && !CheckSettingsInitialized(dataSource))
            {
                foreach (var settingInfo in settingInfos)
                {
                    InitializeSetting(settingInfo);
                }
            }

            foreach (var settingInfo in settingInfos)
            {
                LoadValue(settingInfo);
            }
        }

        private static void LoadValue(SettingInfo settingInfo)
        {
            var value = GetValue(settingInfo);

            if (string.IsNullOrEmpty(value))
            {
                if (!settingInfo.FieldInfo.IsOptional())
                {
                    throw new OptionalException(settingInfo);
                }
                return;
            }

            var converter = GetConverter(settingInfo);

            try
            {
                var obj = converter.DeserializeObject(value, settingInfo.FieldInfo.FieldType, settingInfo.FieldInfo.Contraints());
                settingInfo.FieldInfo.SetValue(null, obj);
            }
            catch (Exception ex)
            {
                throw new ObjectConverterException(settingInfo, ex)
                {
                    Value = value,
                    FromType = typeof(string),
                    ToType = settingInfo.FieldInfo.FieldType
                };
            }
        }

        // gets a value for config field and throws detailed exception if failed
        private static string GetValue(SettingInfo settingInfo)
        {
            try
            {
                var dataSource = DataSources[settingInfo.ConfigType];
                var value = dataSource.Select(settingInfo.SettingPath);
                return value;
            }
            catch (Exception ex)
            {
                throw new DataSourceException(settingInfo, ex);
            }
        }

        #endregion

        /// <summary>
        /// Updates a configuration field.
        /// </summary>
        /// <typeparam name="TField">Type of the field.</typeparam>
        /// <param name="expression">Lambda expression of the field.</param>
        /// <param name="value">Value to be set.</param>
        public static void Update<TField>(Expression<Func<TField>> expression, TField value)
        {
            var settingInfo = new SettingInfo(Utilities.GetMemberInfo(expression));
            UpdateSetting(settingInfo, value);
        }

        public static void UpdateSetting(Type configType, string settingPath, object value)
        {
            var settingInfo = Utilities.FindSettingInfo(configType, settingPath);
            if (settingInfo == null)
            {
                // todo: create a meaningfull exception
                throw new Exception("Setting not found.");
            }

            UpdateSetting(settingInfo, value);
        }

        private static void UpdateSetting(SettingInfo settingInfo, object value, bool isInitialization = false)
        {
            var serializedValue = SerializeValue(value, settingInfo);
            try
            {
                var dataSource = DataSources[settingInfo.ConfigType];
                dataSource.Update(settingInfo.SettingPath, serializedValue);
                if (!isInitialization)
                {
                    settingInfo.FieldInfo.SetValue(null, value);
                }
            }
            catch (ConstraintException<ConstraintAttribute>)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ObjectConverterException(settingInfo, ex)
                {
                    Value = value,
                    FromType = settingInfo.FieldInfo.FieldType,
                    ToType = typeof(string),
                };
            }
        }

        private static bool CheckSettingsInitialized(IDataSource dataSource)
        {
            var settingInitialized = dataSource.Select("__SettingsInitialized");
            var result = false;
            bool.TryParse(settingInitialized, out result);
            return result;
        }

        private static void InitializeSetting(SettingInfo settingInfo)
        {
            UpdateSetting(settingInfo, settingInfo.FieldInfo.GetValue(null), true);
        }

        private static string SerializeValue(object value, SettingInfo settingInfo)
        {
            if (value == null && !settingInfo.FieldInfo.IsNullable())
            {
                throw new OptionalException(settingInfo);
            }

            var converter = GetConverter(settingInfo);

            try
            {
                var serializedValue = converter.SerializeObject(value, settingInfo.FieldInfo.FieldType, settingInfo.SettingConstraints);
                return serializedValue;
            }
            catch (ConstraintException<ConstraintAttribute>)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ObjectConverterException(settingInfo, ex)
                {
                    Value = value,
                    FromType = settingInfo.FieldInfo.FieldType,
                    ToType = typeof(string),
                };
            }
        }

        private static ObjectConverterBase GetConverter(SettingInfo settingInfo)
        {
            var type = GetConverterType(settingInfo.FieldInfo);

            var objectConverter = Converters[type];
            if (objectConverter == null)
            {
                throw new ObjectConverterNotFoundException(settingInfo, type);
            }

            return objectConverter;
        }

        private static Type GetConverterType(FieldInfo fieldInfo)
        {
            var type = fieldInfo.FieldType;

            if (type.BaseType == typeof(Enum))
            {
                return typeof(Enum);
            }

            var objectConverterAttribute = fieldInfo.GetCustomAttribute<ObjectConverterAttribute>(false);
            if (objectConverterAttribute != null)
            {
                return objectConverterAttribute.Type;
            }

            return type;
        }
    }
}