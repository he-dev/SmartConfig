using System;
using System.Collections.Generic;
using System.Globalization;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts value types from and to a string.
    /// </summary>
    public class ValueTypeConverter : ObjectConverter
    {
        public ValueTypeConverter()
            : base(new[]
            {
                typeof(bool),
                //typeof(bool?),
                typeof(char),
                //typeof(char?),
                typeof(short),
                //typeof(short?),
                typeof(int),
                //typeof(int?),
                typeof(long),
                //typeof(long?),
                typeof(float),
                //typeof(float?),
                typeof(double),
                //typeof(double?),
                typeof(decimal),
                //typeof(decimal?)
            })
        {
        }

        public override object DeserializeObject(string value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);

            // nullable types are not supported
            //if (type.IsNullable())
            //{
            //    if (string.IsNullOrEmpty(value))
            //    {
            //        // It is ok to return null for nullable types.
            //        return null;
            //    }
            //    type = Nullable.GetUnderlyingType(type);
            //}

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
                if (!range.IsValid((IComparable)result)) throw new ConstraintException(range, value);
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
