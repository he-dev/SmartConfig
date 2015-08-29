using System;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig
{
    public class DataSourceException : SmartConfigException
    {
        public DataSourceException(IDataSource dataSource, SettingInfo settingInfo, Exception innerException)
            : base(settingInfo, innerException)
        {
            DataSource = dataSource;
        }

        public override string Message => 
            $"An error occured in the " +
            $"DataSourceType = \"{DataSource.GetType().Name}\" " +
            $"SettingPath = \"{SettingInfo.SettingPath}\". " +
            $"See the inner exception for details.";

        public IDataSource DataSource { get; private set; }
    }
}
