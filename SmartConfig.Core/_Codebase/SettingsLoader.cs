using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Logging;
using SmartConfig.Reflection;

namespace SmartConfig
{
    internal class SettingsLoader
    {
        private readonly IObjectConverterCollection _converters;

        private readonly IDataSourceCollection _dataSources;

        public SettingsLoader(IObjectConverterCollection converters, IDataSourceCollection dataSources)
        {
            _converters = converters;
            _dataSources = dataSources;
        }

        /// <summary>
        /// Loads settings for a a configuration from the specified data source.
        /// </summary>
        internal void LoadSettings(ConfigurationInfo configurationInfo)
        {
            Debug.Assert(configurationInfo != null);

            var dataSource = _dataSources[configurationInfo.ConfigType];

            Logger.LogTrace(() => $"Loading \"{configurationInfo.ConfigType.Name}\" from \"{dataSource.GetType().Name}\"...");

            foreach (var settingInfo in configurationInfo.SettingInfos.Values)
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

                    throw new SettingNotOptionalException
                    {
                        ConfigTypeFullName = settingInfo.Configuration.ConfigType.FullName,
                        SettingPath = settingInfo.SettingPath
                    };
                }

                var converter = _converters[settingInfo.ConverterType];
                var obj = converter.DeserializeObject(value, settingInfo.SettingType, settingInfo.SettingConstraints);
                settingInfo.Value = obj;
            }
            catch (Exception ex)
            {
                throw new LoadSettingFailedException(ex)
                {
                    DataSourceTypeName = dataSource.GetType().Name,
                    ConfigTypeFullName = settingInfo.Configuration.ConfigType.FullName,
                    SettingPath = settingInfo.SettingPath
                };
            }
        }
    }
}
