using System;
using System.Diagnostics;
using System.Linq;
using SmartConfig.Collections;

namespace SmartConfig.IO
{
    internal class SettingUpdater
    {
        //public void UpdateSetting(DeclaringTypeName configType, string SettingPath, object value)
        //{
        //    //var Setting = _configurationReflector.FindSettingInfo(configType, SettingPath);
        //    //if (Setting == null)
        //    //{
        //    //    // todo: create a meaningfull exception
        //    //    throw new Exception("Setting not found.");
        //    //}

        //    //UpdateSetting(Setting, value);
        //}

        public static void UpdateSetting(Setting setting, object value, ObjectConverterCollection converters)
        {
            Debug.Assert(setting != null);

            try
            {
                var dataSource = setting.Configuration.DataStore;
                var converter = converters[setting.ConverterType];
                value = converter.SerializeObject(value, setting.Type, setting.Atributes);
                dataSource.Update(setting.Keys, value);
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
