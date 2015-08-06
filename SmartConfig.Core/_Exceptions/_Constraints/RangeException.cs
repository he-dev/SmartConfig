using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occures when is outside of the specified range.
    /// </summary>
    public class RangeException : ConstraintException<RangeAttribute>
    {
        public RangeException(RangeAttribute constraint, string value) : base(constraint, value)
        {
        }

        public override string Message
        {
            get
            {
                return "$Message Min = [$Min], Max = [$Max].".FormatWith(new { base.Message, Constraint.Min, Constraint.Max }, true);
            }
        }
    }
}
