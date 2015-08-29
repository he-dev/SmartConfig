using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        private static readonly DataSourceDictionary DataSources = new DataSourceDictionary();

        /// <summary>
        /// Gets the converters collection that holds all the default converters and allows to add additional ones.
        /// </summary>
        public static ObjectConverterDictionary Converters { get; private set; }

        static SmartConfigManager()
        {
            Converters = new ObjectConverterDictionary
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
        /// <param name="configType">SettingType that is marked with the <c>SmartCofnigAttribute</c> and specifies the configuration.</param>
        /// <param name="dataSource">Data source that provides data.</param>
        public static void Load(Type configType, IDataSource dataSource)
        {
            #region check arguments

            if (configType == null) throw new ArgumentNullException(nameof(configType), "You need to specify a config type.");
            if (dataSource == null) throw new ArgumentNullException(nameof(dataSource), "You need to specify a data source.");

            if (!configType.IsStatic()) throw new InvalidOperationException("'configType' must be a static class.");
            if (configType.GetCustomAttribute<SmartConfigAttribute>() == null) throw new InvalidOperationException("'configType' must be marked with the SmartConfigAttribute.");

            #endregion

            Logger.LogTrace(() => $"Loading \"{configType.Name}\" from \"{dataSource.GetType().Name}\"...");

            DataSources[configType] = dataSource;

            var settingInfos = Utilities.GetSettingInfos(configType).ToList();

            // initialize settings
            if (dataSource.SettingsInitializationEnabled && !CheckSettingsInitialized(dataSource))
            {
                Logger.LogTrace(() => "Initializing settings...");
                foreach (var settingInfo in settingInfos)
                {
                    InitializeSetting(settingInfo);
                }

                var settingInitializedSettingInfo = SettingInfoFactory.CreateSettingsInitializedSettingInfo(configType);
                UpdateSetting(settingInitializedSettingInfo, true, true);
            }

            foreach (var settingInfo in settingInfos)
            {
                LoadSetting(settingInfo);
            }
        }

        // loads a setting from a data source into the correspondig field in the config class
        private static void LoadSetting(SettingInfo settingInfo)
        {
            var value = GetSetting(settingInfo);

            // don't let pass null values to the converter
            if (string.IsNullOrEmpty(value))
            {
                // null is ok if the setting is optional
                if (settingInfo.IsOptional)
                {
                    return;
                }

                // unfortunately this value is invalid, inform the user
                throw new OptionalException(settingInfo);
            }

            try
            {
                var obj = Converters[settingInfo].DeserializeObject(value, settingInfo.SettingType, settingInfo.SettingConstraints);
                settingInfo.Value = obj;
            }
            catch (ConstraintException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ObjectConverterException(value, settingInfo, ex);
            }
        }

        // gets a setting from a data source
        private static string GetSetting(SettingInfo settingInfo)
        {
            var dataSource = DataSources[settingInfo.ConfigType];
            try
            {
                var value = dataSource.Select(settingInfo.SettingPath);
                return value;
            }
            catch (Exception ex)
            {
                throw new DataSourceException(dataSource, settingInfo, ex);
            }
        }

        #endregion

        #region updating
        /// <summary>
        /// Updates a configuration field.
        /// </summary>
        /// <typeparam name="TField">SettingType of the field.</typeparam>
        /// <param name="updateExpression">Lambda updateExpression of the field.</param>
        /// <param name="value">Value to be set.</param>
        public static void Update<TField>(Expression<Func<TField>> updateExpression, TField value)
        {
            if (updateExpression == null) throw new ArgumentNullException(nameof(updateExpression), "You need specify an exprestion for the setting you want to update.");

            var settingInfo = SettingInfo.From(updateExpression);

            Debug.Assert(settingInfo != null);
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
            Debug.Assert(settingInfo != null);

            var serializedValue = SerializeValue(value, settingInfo);
            var dataSource = DataSources[settingInfo.ConfigType];
            try
            {
                dataSource.Update(settingInfo.SettingPath, serializedValue);

                if (isInitialization)
                {
                    return;
                }

                settingInfo.Value = value;
            }
            catch (Exception ex)
            {
                throw new DataSourceException(dataSource, settingInfo, ex);
            }
        }

        #endregion

        private static bool CheckSettingsInitialized(IDataSource dataSource)
        {
            var settingInitialized = dataSource.Select(KeyNames.Internal.SettingsInitializedKeyName);
            bool result;
            bool.TryParse(settingInitialized, out result);

            Logger.LogTrace(() => $"{KeyNames.Internal.SettingsInitializedKeyName} = \"{result}\"");

            return result;
        }

        private static void InitializeSetting(SettingInfo settingInfo)
        {
            Logger.LogTrace(() => $"Initializing SettingPath = \"{settingInfo.SettingPath}\"");
            UpdateSetting(settingInfo, settingInfo.Value, true);
        }

        private static string SerializeValue(object value, SettingInfo settingInfo)
        {
            // don't let pass null value to the converter
            if (value == null)
            {
                return null;
            }

            try
            {
                var serializedValue = Converters[settingInfo].SerializeObject(value, settingInfo.SettingType, settingInfo.SettingConstraints);
                return serializedValue;
            }
            catch (ConstraintException)
            {
                // rethrow constraint violation
                throw;
            }
            catch (Exception ex)
            {
                // add more information about the setting to the generic exception
                throw new ObjectConverterException(value, settingInfo, ex);
            }
        }
    }
}