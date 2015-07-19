using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Marks a type as <c>SmartConfig</c> and allows to set additional options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartConfigAttribute : Attribute
    {
        public SmartConfigAttribute()
        {
            Version = string.Empty;
        }

        /// <summary>
        /// Indicates whether to use config name to prefix element names.
        /// </summary>
        public bool UseConfigName { get; set; }

        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the connection string name if required by the data source.
        /// </summary>
        public string ConnectionStringName { get; set; }
    }
}
