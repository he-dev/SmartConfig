using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class RegularExpressionAttribute : ValueConstraintAttribute
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
            return Regex.IsMatch(value, this, IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        public static implicit operator string(RegularExpressionAttribute pattern)
        {
            return pattern._pattern;
        }
    }
}
