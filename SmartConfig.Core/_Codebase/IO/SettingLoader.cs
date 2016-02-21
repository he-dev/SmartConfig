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
        internal static void LoadSettings(Configuration configuration, ObjectConverterCollection converters)
        {
            Debug.Assert(configuration != null);
            Debug.Assert(converters != null);

            // try to load all settings first
            var settingValues = new Dictionary<string, object>();
            foreach (var setting in configuration.Settings)
            {
                var value = LoadSetting(setting, converters);
                settingValues[setting.Path] = value;
            }

            // now update the model but skip optional null settings
            foreach (var setting in configuration.Settings.Where(setting => !setting.IsOptional || settingValues[setting.Path] != null))
            {
                setting.Value = settingValues[setting.Path];
            }
        }

        // loads a setting from a data source into the correspondig field in the configuration class
        private static object LoadSetting(Setting setting, ObjectConverterCollection converters)
        {
            Debug.Assert(setting != null);
            Debug.Assert(converters != null);

            try
            {
                var data = setting.Configuration.DataStore.Select(setting.Key);

                // don't let pass null data to the converter
                if (data == null)
                {
                    // return null if the setting is optional
                    if (setting.IsOptional)
                    {
                        return null;
                    }

                    // otherwise null is not allowed
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
                    DataStore = setting.Configuration.DataStore.GetType().Name,
                    Configuration = setting.Configuration.Type.FullName,
                    SettingPath = setting.Path
                };
            }
        }
    }
}
