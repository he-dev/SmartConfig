﻿using System;
using System.Diagnostics;
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

            //Logger.LogTrace(() => $"Loading \"{configurationInfo.ConfigType.Name}\" from \"{dataSource.GetType().Name}\"...");

            foreach (var settingInfo in configuration.SettingInfos.Values)
            {
                LoadSetting(settingInfo, configuration.Properties.DataSource, converters);
            }
        }

        // loads a setting from a data source into the correspondig field in the config class
        private static void LoadSetting(SettingInfo settingInfo, IDataSource dataSource, IObjectConverterCollection converters)
        {
            Debug.Assert(settingInfo != null);
            Debug.Assert(dataSource != null);
            Debug.Assert(converters != null);

            try
            {
                var value = dataSource.Select(settingInfo.Keys);

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

                var converter = converters[settingInfo.ConverterType];
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