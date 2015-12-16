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
        public string Pattern { get; set; }

        public RegexOptions RegexOptions { get; set; } = RegexOptions.None;

        public override void Validate(object value)
        {
            if (string.IsNullOrEmpty(Pattern)) { throw new PropertyNotSetException { PropertyName = nameof(Pattern) }; }

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
