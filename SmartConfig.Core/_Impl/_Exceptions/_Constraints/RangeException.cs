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
    public class RangeException : ConstraintException
    {
        public RangeException(RangeAttribute constraint, string value) : base(constraint, value)
        {
        }

        public override string Message
        {
            get
            {
                var constraint = (RangeAttribute)Constraint;
                return "$Message Min = [$Min], Max = [$Max].".FormatWith(new { base.Message, constraint.Min, constraint.Max }, true);
            }
        }
    }
}
