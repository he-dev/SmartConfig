using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.IO
{
    internal class SettingLoader
    {
        /// <summary>
        /// Loads settings for a a configuration from the specified data source.
        /// </summary>
        internal static void LoadSettings(Configuration configuration, ObjectConverterCollection converters)
        {
            Debug.Assert(configuration != null);
            Debug.Assert(converters != null);

            var settingValues = new Dictionary<string, object>();

            foreach (var setting in configuration.Settings)
            {
                var value = LoadSetting(setting, configuration.DataStore, converters);
                settingValues[setting.Path] = value;
            }

            // todo, according to invalid setting handling either skip invalid settings or abort loading

            foreach (var setting in configuration.Settings.Where(setting => !setting.IsOptional || settingValues[setting.Path] != null))
            {
                setting.Value = settingValues[setting.Path];
            }
        }

        // loads a setting from a data source into the correspondig field in the configuration class
        private static object LoadSetting(Setting setting, IDataStore dataSource, ObjectConverterCollection converters)
        {
            Debug.Assert(setting != null);
            Debug.Assert(dataSource != null);
            Debug.Assert(converters != null);

            try
            {
                var data = dataSource.Select(setting.Keys);

                // don't let pass null data to the converter
                if (data == null)
                {
                    // null is ok if the setting is optional
                    if (setting.IsOptional)
                    {
                        return null;
                    }

                    throw new SettingNotOptionalException
                    {
                        ConfigurationType = setting.Configuration.Type.Name,
                        SettingPath = setting.Path,
                        Hint = "Check other keys if you have any custom ones like environment or version."
                    };
                }
            
                var converter = converters[setting.ConverterType];
                var value = converter.DeserializeObject(data, setting.Type, setting.Atributes);
                return value;
            }
            catch (Exception ex)
            {
                throw new LoadSettingFailedException(ex)
                {
                    DataSourceType = dataSource.GetType().Name,
                    ConfigurationType = setting.Configuration.Type.FullName,
                    SettingPath = setting.Path
                };
            }
        }
    }
}
