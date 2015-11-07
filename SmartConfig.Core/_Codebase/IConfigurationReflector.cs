using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    internal interface IConfigurationReflector
    {
        IEnumerable<SettingInfo> GetSettingInfos(Type configType);

        SettingInfo FindSettingInfo(Type configType, string settingPath);
    }
}
