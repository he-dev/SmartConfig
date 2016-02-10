using System;
using System.Diagnostics;
using System.Globalization;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Allows to define an exact date time format.
    /// </summary>
    [DebuggerDisplay("ExpectedFormat = \"{Format}\"")]
    public class DateTimeFormatAttribute : ConverterParameterAttribute
    {
        public DateTimeFormatAttribute(string format)
        {
            if (string.IsNullOrEmpty(format)) { throw new ArgumentNullException(nameof(format)); }
            Format = format;
        }

        /// <summary>
        /// Gets the format set in the constructor.
        /// </summary>
        public string Format { get; }
    }
}
