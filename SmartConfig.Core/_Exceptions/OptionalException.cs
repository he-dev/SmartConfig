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
        public OptionalException(Type configType, string name)
            : base("[$configTypeName]'s field [fieldName] is not optional. If you want it to be optional add the OptionalAttribute.".FormatWith(new { configTypeName = configType.Name, fieldName = name }, true))
        {
            ConfigType = configType;
            FieldName = name;
        }

        /// <summary>
        /// Gets the config type.
        /// </summary>
        public Type ConfigType { get; private set; }

        /// <summary>
        /// Gets the full name of the field that could not be found.
        /// </summary>
        public string FieldName { get; private set; }
    }
}
