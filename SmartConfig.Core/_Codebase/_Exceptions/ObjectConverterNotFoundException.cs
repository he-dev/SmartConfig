using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when an object converter could not be found.
    /// </summary>
    public class ObjectConverterNotFoundException : SmartConfigException
    {
        internal ObjectConverterNotFoundException(SettingInfo settingInfo) : base(settingInfo, null)
        {
        }

        public override string Message
        {
            get
            {
                return "Object converter not found. ConverterType = \"$ConverterTypeName\" ConfigType = \"$ConfigTypeName\" SettingPath = \"$SettingPath\""
                    .FormatWith(new
                    {
                        ConverterTypeName = SettingInfo.ConverterType.Name,
                        ConfigTypeName = SettingInfo.ConfigType.Name,
                        SettingPath = SettingInfo.SettingPath.ToString()
                    }, true);
            }
        }
    }
}
