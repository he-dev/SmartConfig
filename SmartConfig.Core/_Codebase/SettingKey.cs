using System;
using SmartConfig.Data;
using SmartConfig.Filters;

namespace SmartConfig
{
    /// <summary>
    /// Represents information about a key.
    /// </summary>
    public class SettingKey
    {
        public SettingKey(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }
            if (value == null) { throw new ArgumentNullException(nameof(value)); }

            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the key value. This property is optional for the version set via the <c>SmartConfigAttribute</c>.
        /// It is set internaly.
        /// </summary>
        public string Value { get; private set; }
    }
}
