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
    /// Occurs when a field that is not optionl could not be found.
    /// </summary>
    public class OptionalException : Exception
    {
        public override string Message
        {
            get
            {
                return
                    "Field [$fieldName] of [$configTypeName] is not optional. If you want it to be optional add the $optionalAttributeName."
                    .FormatWith(new
                    {
                        FieldName,
                        ConfigTypeName = ConfigType.Name,
                        OptionalAttributeName = typeof(OptionalAttribute).Name
                    }, true);
            }
        }
        /// <summary>
        /// Gets the config type.
        /// </summary>
        public Type ConfigType { get; internal set; }

        /// <summary>
        /// Gets the full name of the field that could not be found.
        /// </summary>
        public string FieldName { get; internal set; }
    }
}
