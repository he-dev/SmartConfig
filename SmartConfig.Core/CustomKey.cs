using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    /// <summary>
    /// Represents information about a key.
    /// </summary>
    /// <typeparam name="TConfigElement"></typeparam>
    public class CustomKey<TConfigElement> where TConfigElement : ConfigElement, new()
    {
        /// <summary>
        /// Gets or sets the name of the key.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the key value. This property is optional for the version set via the <c>SmartConfigAttribute</c>.
        /// It is set internaly.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the filter function for this key.
        /// </summary>
        public FilterByFunc<TConfigElement> Filter { get; set; }

        public CustomKey<TConfigElement> HasName(string keyName)
        {
            Name = keyName;
            return this;
        }

        public CustomKey<TConfigElement> HasValue(string keyValue)
        {
            Value = keyValue;
            return this;
        }

        public CustomKey<TConfigElement> HasFilter(FilterByFunc<TConfigElement> keyFilter)
        {
            Filter = keyFilter;
            return this;
        }
    }
}
