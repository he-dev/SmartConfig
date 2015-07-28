using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts value types from and to a string.
    /// </summary>
    public class ValueTypeConverter : ObjectConverterBase
    {
        public ValueTypeConverter()
            : base(new[]
            {
                typeof(bool),
                typeof(bool?),
                typeof(char),
                typeof(char?),
                typeof(short),
                typeof(short?),
                typeof(int),
                typeof(int?),
                typeof(long),
                typeof(long?),
                typeof(float),
                typeof(float?),
                typeof(double),
                typeof(double?),
                typeof(decimal),
                typeof(decimal?)
            })
        {
        }

        public override object DeserializeObject(string value, Type type, IEnumerable<ValueConstraintAttribute> constraints)
        {
            ValidateType(type);

            if (type.IsNullable())
            {
                if (string.IsNullOrEmpty(value))
                {
                    // It is ok to return null for nullable types.
                    return null;
                }
                type = Nullable.GetUnderlyingType(type);
            }

            var parseMethod = type.GetMethod("Parse", new Type[] { typeof(string), typeof(IFormatProvider) });
            if (parseMethod != null)
            {
                var result = parseMethod.Invoke(null, new object[] { value, CultureInfo.InvariantCulture });
                return result;
            }
            else
            {
                parseMethod = type.GetMethod("Parse", new Type[] { typeof(string) });
                if (parseMethod != null)
                {
                    var result = parseMethod.Invoke(null, new object[] { value });
                    return result;
                }
            }
            throw new Exception("Parse method not found.");
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ValueConstraintAttribute> constraints)
        {
            ValidateType(type);

            if (value == null)
            {
                // It is ok to return null for null objects.
                return null;
            }

            var toStringMethod = type.GetMethod("ToString", new Type[] { typeof(IFormatProvider) });
            if (toStringMethod != null)
            {
                var result = toStringMethod.Invoke(value, new object[] { CultureInfo.InvariantCulture });
                return (string)result;
            }
            else
            {
                toStringMethod = type.GetMethod("ToString", new Type[] { });
                var result = toStringMethod.Invoke(value, null);
                return (string)result;
            }

            throw new Exception("ToString method not found.");
        }
    }
}
