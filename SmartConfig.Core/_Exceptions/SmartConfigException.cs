using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class SmartConfigException : Exception
    {
        public SmartConfigException(SettingInfo settingInfo, Exception innerException)
            : base(string.Empty, innerException)
        {
            SettingInfo = settingInfo;
        }

        public SettingInfo SettingInfo { get; private set; }        
    }
}
