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

        public override string Message => "An error occured in the DataSourceType = \"$DataSourceTypeName\" SettingPath = \"$SettingPath\". See the inner exception for details."
            .FormatWith(new
            {
                DataSourceTypeName = DataSource.GetType().Name,
                SettingPath = SettingInfo.SettingPath.ToString()
            }, true);

        public IDataSource DataSource { get; private set; }
    }
}
