using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    internal static class SettingInfoFactory
    {
        public static SettingInfo CreateSettingsInitializedSettingInfo(Type configType)
        {
            return new SettingInfo(configType, KeyNames.Internal.SettingsInitializedKeyName, typeof(bool));
        }
    }
}
