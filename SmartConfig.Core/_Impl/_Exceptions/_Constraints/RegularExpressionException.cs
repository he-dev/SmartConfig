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
    public class RegularExpressionException : ConstraintException<RegularExpressionAttribute>
    {
        public RegularExpressionException(RegularExpressionAttribute constraint, string value) : base(constraint, value)
        {
        }

        public override string Message
        {
            get
            {
                return "$Message Regex = [$Regex], IgnoreCase = [$IgnoreCase].".FormatWith(new { base.Message, Regex = Constraint, Constraint.IgnoreCase }, true);
            }
        }
    }
}
