using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occures when the value does not match the specified pattern.
    /// </summary>
    public class DateTimeFormatException : ConstraintException<DateTimeFormatAttribute>
    {
        public DateTimeFormatException(DateTimeFormatAttribute constraint, string value) : base(constraint, value)
        {
        }

        public override string Message
        {
            get
            {
                return "$Message Format = [$Format].".FormatWith(new { base.Message, Format = Constraint }, true);
            }
        }
    }
}
