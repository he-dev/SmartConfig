using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Specifies the range for a field.
    /// </summary>
    public class RangeAttribute : ConstraintAttribute
    {
        /// <summary>
        /// Creates a new instance of the <c>RangeAtrributes</c>.
        /// </summary>
        /// <param name="type">SettingType of the values.</param>
        /// <param name="min">Minimum allowed value inclusive. If null there is no minimum.</param>
        /// <param name="max">Maximum allowed value inclusive. If null there is no maximum.</param>
        public RangeAttribute(Type type, string min, string max)
        {
            Type = type;
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Gets the type of the range.
        /// </summary>
        internal Type Type { get; private set; }

        /// <summary>
        /// Gets the minimum value of the range.
        /// </summary>
        internal string Min { get; private set; }

        /// <summary>
        /// Gets the maximum value of the range.
        /// </summary>
        internal string Max { get; private set; }

        public override string ToString()
        {
            return "Type = \"$TypeName\" Min = \"$Min\" Max = \"$Max\"".FormatWith(new { TypeName = Type.Name, Min, Max });
        }

        /// <summary>
        /// Validates the value's range.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsValid(IComparable value)
        {
            var typeConverter = TypeDescriptor.GetConverter(Type);

            var isMin = true;
            if (!string.IsNullOrEmpty(Min))
            {
                var min = (IComparable)typeConverter.ConvertFromString(Min);
                isMin = value.CompareTo(min) >= 0;
            }

            var isMax = true;
            if (!string.IsNullOrEmpty(Max))
            {
                var max = (IComparable)typeConverter.ConvertFromString(Max);
                isMax = value.CompareTo(max) <= 0;
            }

            return isMin && isMax;
        }
    }
}
