using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Provides a regular expression validation.
    /// </summary>
    [DebuggerDisplay("Pattern = \"{Pattern}\" RegexOptions = \"{RegexOptions}\"")]
    public class RegularExpressionAttribute : ConstraintAttribute
    {
        public RegularExpressionAttribute(string pattern, RegexOptions regexOptions = RegexOptions.None)
        {
            if (string.IsNullOrEmpty(pattern)) { throw new ArgumentNullException(nameof(pattern)); }

            Pattern = pattern;
            RegexOptions = regexOptions;
        }

        public string Pattern { get; }

        public RegexOptions RegexOptions { get; }

        public override void Validate(object value)
        {
            var isValid = Regex.IsMatch((string)value, Pattern, RegexOptions);

            if (!isValid)
            {
                throw new RegularExpressionViolationException
                {
                    Pattern = Pattern,
                    RegexOptions = RegexOptions.ToString(),
                    Value = value.ToString()
                };
            }
        }
    }
}
