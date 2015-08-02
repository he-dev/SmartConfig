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
        public RangeException(Type type, string value, string min, string max)
            : base(value, "Value [$value] is not within the specified min [$min] & max [$max] range.".FormatWith(new { value, min, max }, true))
        {
            Value = value;
            Type = type;
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Gets the type of the range.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the minimum value of the range.
        /// </summary>
        public string Min { get; private set; }

        /// <summary>
        /// Gets the maximum value of the range.
        /// </summary>
        public string Max { get; private set; }
    }
}
