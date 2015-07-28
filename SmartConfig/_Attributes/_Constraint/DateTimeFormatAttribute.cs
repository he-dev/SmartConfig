using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Allows to define an exact date time format.
    /// </summary>
    public class DateTimeFormatAttribute : ValueConstraintAttribute
    {
        public DateTimeFormatAttribute(string format)
        {
            Format = format;
        }

        public string Format { get; private set; }

        public override string ToString()
        {
            return Format;
        }

        public static implicit operator string(DateTimeFormatAttribute attribute)
        {
            return attribute.ToString();
        }
    }
}
