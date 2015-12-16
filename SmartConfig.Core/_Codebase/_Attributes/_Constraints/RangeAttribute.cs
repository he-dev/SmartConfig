using System;
using System.ComponentModel;
using System.Diagnostics;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Specifies the range for a field.
    /// </summary>
    [DebuggerDisplay("Type = \"{Type.Name}\" Min = \"{Min}\" Max = \"{Max}\"")]
    public class RangeAttribute : ConstraintAttribute
    {
        /// <summary>
        /// Gets or sets the type of the range.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the minimum value of the range.
        /// </summary>
        public string Min { get; set; }

        /// <summary>
        /// Gets or sets the maximum value of the range.
        /// </summary>
        public string Max { get; set; }

        /// <summary>
        /// Validates the value's range.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override void Validate(object value)
        {
            var comparable = value as IComparable;
            if (comparable == null)
            {
                throw new ArgumentException(null, nameof(value));
            }

            if (Type == null)
            {
                throw new PropertyNotSetException { PropertyName = nameof(Type) };
            }

            if (string.IsNullOrEmpty(Min) && string.IsNullOrEmpty(Max))
            {
                throw new PropertyNotSetException { PropertyName = $"{nameof(Min)} and/or {nameof(Max)}" };
            }

            var typeConverter = TypeDescriptor.GetConverter(Type);

            var isMin = true;
            if (!string.IsNullOrEmpty(Min))
            {
                var min = (IComparable)typeConverter.ConvertFromString(Min);
                isMin = comparable.CompareTo(min) >= 0;
            }

            var isMax = true;
            if (!string.IsNullOrEmpty(Max))
            {
                var max = (IComparable)typeConverter.ConvertFromString(Max);
                isMax = comparable.CompareTo(max) <= 0;
            }

            if (!(isMin && isMax))
            {
                throw new RangeViolationException
                {
                    Value = value.ToString(),
                    RangeTypeName = Type.Name,
                    Min = Min,
                    Max = Max
                };
            }
        }
    }
}
