using System;
using System.Diagnostics;
using System.Linq;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Logging;
using SmartConfig.Reflection;

namespace SmartConfig.IO
{
    internal class SettingLoader
    {
        /// <summary>
        /// Loads settings for a a configuration from the specified data source.
        /// </summary>
        internal static void LoadSettings(ConfigurationInfo configuration, IObjectConverterCollection converters)
        {
            Debug.Assert(configuration != null);
            Debug.Assert(converters != null);

            //Logger.LogTrace(() => $"Loading \"{configurationInfo.ConfigurationType.Name}\" from \"{dataSource.GetType().Name}\"...");

            foreach (var settingInfo in configuration.SettingInfos)
            {
                LoadSetting(settingInfo, configuration.ConfigurationPropertyGroup.DataSource, converters);
            }
        }

        // loads a setting from a data source into the correspondig field in the configuration class
        private static void LoadSetting(SettingInfo settingInfo, IDataSource dataSource, IObjectConverterCollection converters)
        {
            Debug.Assert(settingInfo != null);
            Debug.Assert(dataSource != null);
            Debug.Assert(converters != null);

            try
            {
                var value = dataSource.Select(settingInfo.Keys);

                // don't let pass null values to the converter
                if (value == null)
                {
                    // null is ok if the setting is optional
                    if (settingInfo.IsOptional)
                    {
                        return;
                    }

                    throw new SettingNotOptionalException
                    {
                        ConfigTypeFullName = settingInfo.Configuration.ConfigurationType.FullName,
                        SettingPath = settingInfo.SettingPath
                    };
                }
            
                var converter = converters[settingInfo.ConverterType];
                var obj = converter.DeserializeObject(value, settingInfo.SettingType, settingInfo.SettingCustomAttributes);

                settingInfo.Value = obj;
            }
            catch (Exception ex)
            {
                throw new LoadSettingFailedException(ex)
                {
                    DataSourceTypeName = dataSource.GetType().Name,
                    ConfigTypeFullName = settingInfo.Configuration.ConfigurationType.FullName,
                    SettingPath = settingInfo.SettingPath
                };
            }
        }
    }
}
