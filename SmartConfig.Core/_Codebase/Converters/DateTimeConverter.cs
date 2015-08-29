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
    /// Converts <c>DateTime</c> from and to a string. Without this attribute the invariant culture is used.
    /// </summary>
    public class DateTimeConverter : ObjectConverter
    {
        public DateTimeConverter()
            : base(new[] { typeof(DateTime) })
        {
        }

        public override object DeserializeObject(string value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);

            DateTime result;
            constraints.Check<DateTimeFormatAttribute>(format =>
            {
                if (!format.TryParseExact(value, out result))
                {
                    throw new ConstraintException(format, value);
                }
            });

            result = DateTime.Parse(value, CultureInfo.InvariantCulture);
            return result;
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);

            // there's nothing to serialize
            if (value == null)
            {
                return null;
            }

            var result = string.Empty;
            constraints.Check<DateTimeFormatAttribute>(format =>
            {
                result = ((DateTime)value).ToString(format.Format);
            });

            if (string.IsNullOrEmpty(result))
            {
                result = ((DateTime)value).ToString(CultureInfo.InvariantCulture);
            }
            return result;
        }
    }
}
