using System;
using System.Collections.Generic;
using System.Globalization;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts numeric types from and to a string.
    /// </summary>
    public class NumericConverter : ObjectConverter
    {
        public NumericConverter()
            : base(new[]
            {
                // integral types
                typeof(sbyte),
                typeof(byte),
                typeof(char),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                // floating-point types
                typeof(float),
                typeof(double),
                // decimal
                typeof(decimal),
            })
        {
        }

        public override object DeserializeObject(string value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);

            object result = null;

            var parseMethod = type.GetMethod("Parse", new[] { typeof(string), typeof(IFormatProvider) });
            if (parseMethod != null)
            {
                result = parseMethod.Invoke(null, new object[] { value, CultureInfo.InvariantCulture });
            }
            else
            {
                parseMethod = type.GetMethod("Parse", new[] { typeof(string) });
                if (parseMethod != null)
                {
                    result = parseMethod.Invoke(null, new object[] { value });
                }
            }

            constraints.Check<RangeAttribute>(range =>
            {
                if (!range.IsValid((IComparable)result)) throw new RangeViolationException
                {
                    Range = range.ToString(),
                    Value =  value                
                };
            });

            return result;
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);           

            var toStringMethod = type.GetMethod("ToString", new[] { typeof(IFormatProvider) });
            if (toStringMethod != null)
            {
                var result = toStringMethod.Invoke(value, new object[] { CultureInfo.InvariantCulture });
                return (string)result;
            }
            else
            {
                toStringMethod = type.GetMethod("ToString", new Type[] { });
                if (toStringMethod != null)
                {
                    var result = toStringMethod.Invoke(value, null);
                    return (string)result;
                }
            }

            throw new Exception("ToString method not found.");
        }
    }
}
