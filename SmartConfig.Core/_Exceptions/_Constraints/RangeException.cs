using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class RangeException : ConstraintException
    {
        public RangeException(Type type, string value, string min, string max)
            : base(value, string.Format("Value [{0}] is not within the specified min [{1}] & max [{2}] range.", value, min, max))
        {
            Value = value;
            Type = type;
            Min = min;
            Max = max;
        }

        public Type Type { get; private set; }

        public string Min { get; private set; }

        public string Max { get; private set; }
    }
}
