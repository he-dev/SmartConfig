using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig
{
    public class SerializationException : SmartConfigException
    {
        public SerializationException( object value, SettingInfo settingInfo, Exception innerException)
            : base(settingInfo, innerException)
        {
            Value = value;
        }

        public override string Message
        {
            get
            {
                return
                    "Value could not be be serialized from [$typeName]. See inner exeption for details."
                    .FormatWith(new { typeName = SettingInfo.SettingType.Name, }, true);
            }
        }

        public object Value { get; private set; }
    }
}
