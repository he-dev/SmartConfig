using System;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig
{
    public class LoadSettingException : SmartConfigException
    {
        public LoadSettingException(SettingInfo settingInfo, IDataSource dataSource, Exception innerException)
            : base(settingInfo, innerException)
        {
            DataSource = dataSource;
        }

        public override string Message => 
            $"An error occured while loading setting: " +
            $"DataSourceType = \"{DataSource.GetType().Name}\" " +
            $"SettingPath = \"{SettingInfo.SettingPath}\". " +
            $"See the inner exception for details.";

        public IDataSource DataSource { get; }
    }
}
