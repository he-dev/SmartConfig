using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class DateTimeFormatAttribute : ValueContraintAttribute
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
