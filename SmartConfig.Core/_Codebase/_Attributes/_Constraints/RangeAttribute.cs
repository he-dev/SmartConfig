using System;
using System.ComponentModel;
using System.Diagnostics;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Specifies the range for a field.
    /// </summary>
    [DebuggerDisplay("DeclaringTypeName = \"{Type.Name}\" Min = \"{Min}\" Max = \"{Max}\"")]
    public class RangeAttribute : ConstraintAttribute
    {
        public RangeAttribute(Type type, string min, string max)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (string.IsNullOrEmpty(min) && string.IsNullOrEmpty(max))
            {
                throw new PropertyNotSetException
                {
                    PropertyName = $"{nameof(Min)} and/or {nameof(Max)}",
                    Hint = $"At least one of the properties {nameof(Min)} or {nameof(Max)} must be set."
                };
            }

            Type = type;
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Gets or sets the type of the range.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets or sets the minimum value of the range.
        /// </summary>
        public string Min { get; }

        /// <summary>
        /// Gets or sets the maximum value of the range.
        /// </summary>
        public string Max { get; }

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
                throw new ArgumentException($"Value argument must be of type ${nameof(IComparable)}.", nameof(value));
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
