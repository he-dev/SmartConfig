using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when an object converter could not convert a value. See the inner exception for more details.
    /// </summary>
    public class ObjectConverterException : Exception
    {
        public ObjectConverterException(Type configType, string elementName, Exception innerException)
            : base("See inner excption for details", innerException)
        {
            ConfigType = configType;
            ElementName = elementName;
        }

        /// <summary>
        /// Gets the config type where the exception occured.
        /// </summary>
        public Type ConfigType { get; private set; }

        /// <summary>
        /// Gets the full element name that caused the exception.
        /// </summary>
        public string ElementName { get; private set; }
    }
}
