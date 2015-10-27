using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig
{
    internal class SettingsLoader
    {
        private readonly IConfigReflector _configReflector;

        private readonly IObjectConverterCollection _converters;

        private readonly IDataSourceCollection _dataSources;

        public SettingsLoader(
            IConfigReflector configReflector, 
            IObjectConverterCollection converters, 
            IDataSourceCollection dataSources)
        {
            _configReflector = configReflector;
            _converters = converters;
            _dataSources = dataSources;
        }

        /// <summary>
        /// Loads settings for a a configuration from the specified data source.
        /// </summary>
        /// <param name="configType">SettingType that is marked with the <c>SmartCofnigAttribute</c> and specifies the configuration.</param>
        internal void LoadSettings(Type configType)
        {
            Debug.Assert(configType != null);

            var dataSource = _dataSources[configType];

            Logger.LogTrace(() => $"Loading \"{configType.Name}\" from \"{dataSource.GetType().Name}\"...");

            var settingInfos = _configReflector.GetSettingInfos(configType).ToList();

            foreach (var settingInfo in settingInfos)
            {
                LoadSetting(settingInfo, dataSource);
            }
        }

        // loads a setting from a data source into the correspondig field in the config class
        private void LoadSetting(SettingInfo settingInfo, IDataSource dataSource)
        {
            try
            {
                var value = dataSource.Select(settingInfo.SettingPath);

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

                var converter = _converters[settingInfo.ConverterType];
                var obj = converter.DeserializeObject(value, settingInfo.SettingType, settingInfo.SettingConstraints);
                settingInfo.Value = obj;
            }
            catch (Exception ex)
            {
                throw new LoadSettingException(settingInfo, dataSource, ex);
            }
        }
    }
}
