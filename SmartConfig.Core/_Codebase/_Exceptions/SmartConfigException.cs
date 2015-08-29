using System;

namespace SmartConfig
{
    public class SmartConfigException : Exception
    {
        protected SmartConfigException(SettingInfo settingInfo, Exception innerException)
            : base(string.Empty, innerException)
        {
            SettingInfo = settingInfo;
        }

        public SettingInfo SettingInfo { get; private set; }        
    }
}
