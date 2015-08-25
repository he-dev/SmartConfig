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
        public RangeException(ConstraintAttribute constraint, string value) : base(constraint, value)
        {
        }

        //public override string Message
        //{
        //    get
        //    {
        //        return "$Message $Range".FormatWith(new { base.Message, Range = ((RangeAttribute)Constraint).ToString() }, true);
        //    }
        //}
    }
}
