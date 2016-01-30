using System;
using System.Diagnostics;
using System.Linq;
using SmartConfig.Collections;
using SmartConfig.Reflection;

namespace SmartConfig.IO
{
    internal class SettingUpdater
    {
        //public void UpdateSetting(DeclaringTypeName configType, string settingPath, object value)
        //{
        //    //var settingInfo = _configurationReflector.FindSettingInfo(configType, settingPath);
        //    //if (settingInfo == null)
        //    //{
        //    //    // todo: create a meaningfull exception
        //    //    throw new Exception("Setting not found.");
        //    //}

        //    //UpdateSetting(settingInfo, value);
        //}

        public static void UpdateSetting(SettingInfo settingInfo, object value, IObjectConverterCollection converters)
        {
            Debug.Assert(settingInfo != null);

            try
            {
                var dataSource = settingInfo.Configuration.ConfigurationPropertyGroup.DataSource;
                var converter = converters[settingInfo.ConverterType];
                value = converter.SerializeObject(value, settingInfo.SettingType, settingInfo.SettingCustomAttributes);
                dataSource.Update(settingInfo.Keys, value);
            }
            catch (Exception ex)
            {
                throw new UpdateSettingFailedException(ex)
                {
                    DataSourceTypeName = settingInfo.Configuration.ConfigurationPropertyGroup.DataSource.GetType().Name,
                    ConfigTypeFullName = settingInfo.Configuration.ConfigurationType.FullName,
                    SettingPath = settingInfo.SettingPath
                };
            }
        }
    }
}
