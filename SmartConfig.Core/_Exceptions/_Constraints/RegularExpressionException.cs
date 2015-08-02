using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occures when the value does not match the specified pattern.
    /// </summary>
    public class RegularExpressionException : ConstraintException
    {
        public RegularExpressionException(string value, string pattern)
            : base(value, "Value [$value] does not match the pattern [$pattern].".FormatWith(new { value, pattern }, true))
        {
            Value = value;
            Pattern = pattern;
        }

        /// <summary>
        /// Gets the pattern used to validate the value.
        /// </summary>
        public string Pattern { get; private set; }
    }
}
