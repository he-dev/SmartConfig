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
        internal ObjectConverterNotFoundException(ConfigFieldInfo configFieldInfo, Type converterType) : base(configFieldInfo, null)
        {
            ConverterType = converterType;
        }

        public override string Message
        {
            get
            {
                return "Object converter [$ConverterTypeName] for [$ConfigTypeName]'s field [$FieldPath] not found."
                    .FormatWith(new
                    {
                        ConverterTypeName = ConverterType.Name,
                        ConfigTypeName = ConfigFieldInfo.ConfigType.Name,
                        FieldFullName = ConfigFieldInfo.FieldPath
                    }, true);
            }
        }

        public Type ConverterType { get; private set; }
    }
}
