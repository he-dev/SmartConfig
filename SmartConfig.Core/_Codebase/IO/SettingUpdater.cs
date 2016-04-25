using System;
using System.Diagnostics;
using System.Linq;
using SmartUtilities.Collections;
using SmartUtilities.ObjectConverters.DataAnnotations;

namespace SmartConfig.IO
{
    internal class SettingUpdater
    {
        //public void UpdateSetting(SettingType configType, string SettingPath, object value)
        //{
        //    //var Setting = _configurationReflector.FindSettingInfo(configType, SettingPath);
        //    //if (Setting == null)
        //    //{
        //    //    // todo: create a meaningfull exception
        //    //    throw new Exception("Setting not found.");
        //    //}

        //    //UpdateSetting(Setting, value);
        //}

        public static void UpdateSetting(Setting setting) //, object value, ObjectConverterCollection converters)
        {
            Debug.Assert(setting != null);

            try
            {
                var dataSource = setting.Configuration.DataStore;

                var serializationType = dataSource.GetSerializationDataType(setting.NormalizedType);

                var converter = setting.Configuration.Converters[setting.NormalizedType];
                var serializedValue = converter.SerializeObject(setting.Value, serializationType, setting.Atributes.OfType<ValidationAttribute>());
                dataSource.Update(setting.Key, serializedValue);
            }
            catch (Exception ex)
            {
                throw new UpdateSettingFailedException(ex)
                {
                    DataStore = setting.Configuration.DataStore.GetType().Name,
                    Configuration = setting.Configuration.Type.Name,
                    SettingPath = setting.Path
                };
            }
        }
    }
}
