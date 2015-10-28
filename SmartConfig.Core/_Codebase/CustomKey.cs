using System;
using SmartConfig.Data;

namespace SmartConfig
{
    /// <summary>
    /// Represents information about a key.
    /// </summary>
    public class CustomKey
    {
        private string _name;
        private string _value;
        private FilterByFunc _filter;

        public CustomKey() { }

        public CustomKey(string name, string value, FilterByFunc filter)
        {
            Name = name;
            Value = value;
            Filter = filter;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(Name));
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the key value. This property is optional for the version set via the <c>SmartConfigAttribute</c>.
        /// It is set internaly.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("Value");
                _value = value;
            }
        }

        /// <summary>
        /// Gets or sets the filter function for this key.
        /// </summary>
        public FilterByFunc Filter
        {
            get { return _filter; }
            set
            {
                if (value == null) throw new ArgumentNullException("Filter");
                _filter = value;
            }
        }
    }
}
