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
    /// Provides a regular expression validation.
    /// </summary>
    public class RegularExpressionAttribute : ConstraintAttribute
    {
        private readonly string _pattern;

        public RegularExpressionAttribute(string pattern, bool ignoreCase = false)
        {
            _pattern = pattern;
            IgnoreCase = ignoreCase;
        }

        public bool IgnoreCase { get; private set; }

        public bool IsMatch(string value)
        {
            return Regex.IsMatch(value, _pattern, IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        public override string ToString()
        {
            return "Pattern = \"$Pattern\" IgnoreCase = \"$IgnoreCase\"".FormatWith(new { Pattern = _pattern, IgnoreCase = IgnoreCase.ToString() });
        }

        //public static implicit operator string(RegularExpressionAttribute pattern)
        //{
        //    return pattern._pattern;
        //}
    }
}
