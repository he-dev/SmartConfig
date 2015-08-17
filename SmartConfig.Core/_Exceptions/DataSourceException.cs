using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    public class DataSourceException : SmartConfigException
    {
        public DataSourceException(ConfigFieldInfo configFieldInfo, Exception innerException)
            : base(configFieldInfo, innerException)
        {

        }

        public override string Message
        {
            get
            {
                return 
                    "An error occured while reading the data for: Config = [$ConfigType], FieldInfo = [$FieldName]. See the inner exception for details."
                    .FormatFrom(ConfigFieldInfo, true);
            }
        }
    }
}
