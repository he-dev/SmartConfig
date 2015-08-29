using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public override string Message
        {
            get
            {
                return 
                    "An error occured in the DataSourceType = \"$DataSourceTypeName\" SettingPath = \"$SettingPath\". See the inner exception for details."
                    .FormatWith(new
                    {
                        DataSourceTypeName = DataSource.GetType().Name,
                        SettingPath = SettingInfo.SettingPath.ToString()
                    }, true);
            }
        }

        public IDataSource DataSource { get; private set; }
    }
}
