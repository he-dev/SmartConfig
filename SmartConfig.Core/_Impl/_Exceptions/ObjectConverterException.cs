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
        public ObjectConverterException(SettingInfo settingInfo, Exception innerException)
            : base(settingInfo, innerException)
        {
        }

        public override string Message
        {
            get
            {
                return
                    "Value could not be be converted from [$fromType] into [$toType]. See inner exeption for details."
                    .FormatWith(new
                    {
                        fromType = FromType.Name,
                        toType = ToType.Name
                    }, true);
            }
        }

        public object Value { get; internal set; }

        public Type FromType { get; internal set; }

        public Type ToType { get; internal set; }
    }
}
