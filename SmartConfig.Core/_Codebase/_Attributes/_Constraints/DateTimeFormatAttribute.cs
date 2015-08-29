using System;
using System.Diagnostics;
using System.Globalization;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Allows to define an exact date time format.
    /// </summary>
    [DebuggerDisplay("Format = \"{Format}\"")]
    public class DateTimeFormatAttribute : ConstraintAttribute
    {
        public DateTimeFormatAttribute(string format)
        {
            Format = format;
        }

        /// <summary>
        /// Gets the format set in the constructor.
        /// </summary>
        public string Format { get; private set; }

        public override string Properties => $"Format = \"{Format}\"";

        public bool TryParseExact(string value, out DateTime result)
        {
            return DateTime.TryParseExact(value, Format, null, DateTimeStyles.None, out result);
        }

        public override string ToString()
        {
            return Format;
        }

        //public static implicit operator string (DateTimeFormatAttribute attribute)
        //{
        //    return attribute.ToString();
        //}
    }
}
