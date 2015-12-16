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
        public SettingKey(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }
            if (value == null) { throw new ArgumentNullException(nameof(value)); }

            Name = name;
            Value = value;
        }

        internal SettingKey(string name, params string[] path)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }
            if (path == null) { throw new ArgumentNullException(nameof(path)); }

            Name = name;
            Value = new SettingPath(path);
        }

        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the key value. This property is optional for the version set via the <c>SmartConfigAttribute</c>.
        /// It is set internaly.
        /// </summary>
        public object Value { get; private set; }
    }
}
