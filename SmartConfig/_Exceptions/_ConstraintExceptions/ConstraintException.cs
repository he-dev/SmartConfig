using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class ConstraintException : Exception
    {
        public ConstraintException(object value, string message)
            : base(message)
        //: base(string.Format("String [{0}] does not match the pattern [{1}].", value, pattern))
        {
            Value = value;
        }

        public object Value { get; protected set; }
    }
}
