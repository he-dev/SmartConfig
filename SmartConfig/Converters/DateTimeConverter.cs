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
    public class DateTimeConverter : ObjectConverterBase
    {
        public DateTimeConverter()
            : base(new[] { typeof(DateTime) })
        {
        }

        public override object DeserializeObject(string value, Type type, IEnumerable<ValueConstraintAttribute> constraints)
        {
            ValidateType(type);

            var dateTimeFormat = constraints.OfType<DateTimeFormatAttribute>().SingleOrDefault();
            if (dateTimeFormat != null)
            {
                var result = DateTime.ParseExact(value, dateTimeFormat, null);
                return result;
            }
            else
            {
                var result = DateTime.Parse(value, CultureInfo.InvariantCulture);
                return result;
            }
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ValueConstraintAttribute> constraints)
        {
            ValidateType(type);

            if (value == null && !type.IsNullable())
            {
                throw new ArgumentNullException("value", "This field does not allow null values.");
            }

            var dateTimeFormat = constraints.OfType<DateTimeFormatAttribute>().SingleOrDefault();
            if (dateTimeFormat != null)
            {
                var result = ((DateTime)value).ToString(dateTimeFormat);
                return result;
            }
            else
            {
                var result = ((DateTime)value).ToString(CultureInfo.InvariantCulture);
                return result;
            }
        }
    }
}
