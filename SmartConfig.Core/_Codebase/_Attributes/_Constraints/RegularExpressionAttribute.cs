using System.Diagnostics;
using System.Text.RegularExpressions;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Provides a regular expression validation.
    /// </summary>
    [DebuggerDisplay("Pattern = \"{_pattern}\" IgnoreCase = \"{IgnoreCase}\"")]
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

        public override string Properties => $"Pattern = \"{_pattern}\" IgnoreCase = \"{IgnoreCase}\"";

        public override string ToString()
        {
            return _pattern;
        }

        //public static implicit operator string(RegularExpressionAttribute pattern)
        //{
        //    return pattern._pattern;
        //}
    }
}
