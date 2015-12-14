using System;
using System.Diagnostics;
using SmartConfig.Collections;
using SmartConfig.Reflection;

namespace SmartConfig.IO
{
    internal class SettingUpdater
    {
        //public void UpdateSetting(Type configType, string settingPath, object value)
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
            //Debug.Assert(settingInfo != null);

            //try
            //{
            //    var converter = converters[settingInfo.ConverterType];
            //    var serializedValue = converter.SerializeObject(value, settingInfo.SettingType, settingInfo.SettingConstraints);
            //    var dataSource = settingInfo.configuration.ConfigurationProperties.DataSource;
            //    dataSource.Update(settingInfo.SettingPath, serializedValue);
            //}
            //catch (Exception ex)
            //{
            //    //throw new DataSourceException(dataSource, settingInfo, ex);
            //}
        }        
    }
}
