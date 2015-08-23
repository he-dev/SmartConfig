using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Defines an additional key for filtering config elements.
    /// </summary>
    /// <remarks>Config elements are always selected only by name. 
    /// After you get a list of all the values it is possible to use a filter function to narrow down the elements.</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FieldKeyAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <c>FieldKeyAttribute</c> from a "Key=Value" string.
        /// </summary>
        /// <param name="keyValue"></param>
        public FieldKeyAttribute(string keyValue)
        {
            var pattern = @"(?<Key>$identifierPattern)=(?<Value>.+)".FormatWith(new { Constants.IdentifierPattern }, true);
            var keyValueMatch = Regex.Match(keyValue, pattern, RegexOptions.IgnoreCase);
            if (!keyValueMatch.Success)
            {
                throw new ArgumentException(
                    "Value [$keyValue] is not a valid field \"Key=Value\" pair.".FormatWith(new { keyValue }, true),
                    "keyValue");
            }

            Key = keyValueMatch.Groups["Key"].Value;
            Value = keyValueMatch.Groups["Value"].Value;
        }

        public FieldKeyAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }
    }
}
