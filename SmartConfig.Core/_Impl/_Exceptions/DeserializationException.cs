using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig
{
    public class DeserializationException : SmartConfigException
    {
        public DeserializationException(string value, SettingInfo settingInfo, Exception innerException)
            : base(settingInfo, innerException)
        {
            Value = value;
        }

        public override string Message
        {
            get
            {
                return
                    "Value could not be be deserialized to [$typeName]. See inner exeption for details."
                    .FormatWith(new { typeName = SettingInfo.SettingType.Name, }, true);
            }
        }

        public string Value { get; private set; }
    }
}
