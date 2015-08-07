﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when a field that is not optionl could not be found.
    /// </summary>
    public class OptionalException : SmartConfigException
    {
        internal OptionalException(ConfigFieldInfo configFieldInfo) : base(configFieldInfo, null) { }

        public override string Message
        {
            get
            {
                return
                    "[$ConfigTypeName]'s field [$FieldName] is not optional. If you want it to be optional add the $optionalAttributeName."
                    .FormatWith(new
                    {
                        ConfigTypeName = ConfigFieldInfo.ConfigType.Name,
                        ConfigFieldInfo.FieldFullName,
                        OptionalAttributeName = typeof(OptionalAttribute).Name
                    }, true);
            }
        }
    }
}
