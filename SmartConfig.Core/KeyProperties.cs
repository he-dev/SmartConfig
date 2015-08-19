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
    public class KeyProperties
    {
        /// <summary>
        /// Gets or sets the key value. This property is optional for the version set via the <c>SmartConfigAttribute</c>.
        /// It is set internaly.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the filter function for this key.
        /// </summary>
        public FilterByFunc Filter { get; set; }     
    }
}
