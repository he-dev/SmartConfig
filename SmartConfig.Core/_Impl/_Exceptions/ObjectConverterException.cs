using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when an object converter could not convert a value. See the inner exception for more details.
    /// </summary>
    public class ObjectConverterException : SmartConfigException
    {
        public ObjectConverterException(object value, SettingInfo settingInfo, Exception innerException)
            : base(settingInfo, innerException)
        {
            Value = value;
        }

        public override string Message
        {
            get
            {
                return
                    "Error converting SettingPath = \"$SettingPath\" SettingType = \"$SettingType\". See inner exeption for details."
                    .FormatWith(new
                    {
                        SettingPath = SettingInfo.SettingPath.ToString(),
                        SettingType = SettingInfo.SettingType.Name
                    }, true);
            }
        }

        public object Value { get; private set; }
    }
}
