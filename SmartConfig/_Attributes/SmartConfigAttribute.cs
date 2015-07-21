using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Marks a type as <c>SmartConfig</c> and allows to set additional options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartConfigAttribute : Attribute
    {
        private string name;

        public SmartConfigAttribute()
        {
            Version = string.Empty;
        }

        /// <summary>
        /// Gets or sets a custom config name. The name must be a valid CLR identifier.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    name = value;
                }

                // https://regex101.com/r/dW3gF3/1
                if (!Regex.IsMatch(value, @"^[A-Z_][A-Z0-9_]+", RegexOptions.IgnoreCase))
                {
                    throw new ArgumentOutOfRangeException("Name", "Config name must a valid CLR identifier.");
                }

                name = value;
            }
        }

        public string Version { get; set; }
    }
}
