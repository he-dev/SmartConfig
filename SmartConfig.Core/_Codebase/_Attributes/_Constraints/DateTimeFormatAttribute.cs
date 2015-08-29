using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Allows to define an exact date time format.
    /// </summary>
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

        public bool TryParseExact(string value, out DateTime result)
        {
            return DateTime.TryParseExact(value, Format, null, DateTimeStyles.None, out result);
        }

        public override string ToString()
        {
            return "Format = \"$Format\"".FormatWith(new { Format });
        }

        //public static implicit operator string (DateTimeFormatAttribute attribute)
        //{
        //    return attribute.ToString();
        //}
    }
}
