using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Collections;
using SmartConfig.Converters;
using SmartConfig.Data;

namespace SmartConfig
{
    internal class SettingsUpdater
    {
        private readonly IConfigurationReflector _configurationReflector;

        private readonly IObjectConverterCollection _objectConverters;

        private readonly IDataSourceCollection _dataSources;

        public SettingsUpdater(
            IConfigurationReflector configurationReflector,
            IObjectConverterCollection objectConverters,
            IDataSourceCollection dataSources)
        {
            _configurationReflector = configurationReflector;
            _objectConverters = objectConverters;
            _dataSources = dataSources;
        }

        public void UpdateSetting(Type configType, string settingPath, object value)
        {
            var settingInfo = _configurationReflector.FindSettingInfo(configType, settingPath);
            if (settingInfo == null)
            {
                // todo: create a meaningfull exception
                throw new Exception("Setting not found.");
            }

            UpdateSetting(settingInfo, value);
        }

        public void UpdateSetting(SettingInfo settingInfo, object value)
        {
            Debug.Assert(settingInfo != null);

            var serializedValue = SerializeValue(value, settingInfo);
            var dataSource = _dataSources[settingInfo.ConfigType];
            try
            {
                dataSource.Update(settingInfo.SettingPath, serializedValue);
            }
            catch (Exception ex)
            {
                throw new DataSourceException(dataSource, settingInfo, ex);
            }
        }

        private string SerializeValue(object value, SettingInfo settingInfo)
        {
            // don't let pass null value to the converter
            if (value == null)
            {
                return null;
            }

            try
            {
                var converter = _objectConverters[settingInfo.ConverterType];
                var serializedValue = converter.SerializeObject(value, settingInfo.SettingType, settingInfo.SettingConstraints);
                return serializedValue;
            }
            catch (ConstraintException)
            {
                // rethrow constraint violation
                throw;
            }
            catch (Exception ex)
            {
                // add more information about the setting to the generic exception
                throw new ObjectConverterException(value, settingInfo, ex);
            }
        }
    }
}
