using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Defines a custom key for filtering config elements.
    /// </summary>
    /// <remarks>Config elements are always selected only by name. 
    /// After you get a list of all the values it is possible to use a filter function to narrow down the elements.</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CustomKeyAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <c>CustomKeyAttribute</c> from a "Key=Value" string.
        /// </summary>
        /// <param name="keyValue"></param>
        public CustomKeyAttribute(string keyValue)
        {
            var parts = keyValue.Split('=');
            if (parts.Length != 2)
            {
                throw new Exception("Invalid constant");
            }

            Key = parts[0].Trim();
            Value = parts[1].Trim();
        }

        public CustomKeyAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }
    }
}
