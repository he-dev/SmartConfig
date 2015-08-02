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
    /// Occurs when a config element could not be found.
    /// </summary>
    public class ConfigElementNotFounException : Exception
    {
        public ConfigElementNotFounException(Type configType, string elementName)
            : base("Config element [$elementName] not found in [$name]. If you want this field to be optional add the OptionalAttribute.".FormatWith(new { elementName, configType.Name }))
        {
            ConfigType = configType;
            ElementName = elementName;
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        public Type ConfigType { get; private set; }

        /// <summary>
        /// Gets the full path of the config element.
        /// </summary>
        public string ElementName { get; private set; }
    }
}
