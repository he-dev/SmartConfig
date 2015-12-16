using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts <c>DateTime</c> from and to a string. Without this attribute the invariant culture is used.
    /// </summary>
    public class DateTimeConverter : ObjectConverter
    {
        public DateTimeConverter() : base(new[] { typeof(DateTime) }) { }

        public override object DeserializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (value.GetType() == type) { return value; }

            var dateTimeFormatAttribute = attributes.OfType<DateTimeFormatAttribute>().SingleOrDefault();
            if (dateTimeFormatAttribute != null)
            {
                DateTime dateTime;
                if (!DateTime.TryParseExact((string)value, dateTimeFormatAttribute.Format, null, DateTimeStyles.None, out dateTime))
                {
                    throw new DateTimeFormatViolationException
                    {
                        Value = value.ToString(),
                        Format = dateTimeFormatAttribute.Format
                    };
                }
                return dateTime;
            }

            var result = DateTime.Parse((string)value, CultureInfo.InvariantCulture);
            return result;
        }

        public override object SerializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (value.GetType() == type) { return value; }

            var dateTimeFormatAttribute = attributes.OfType<DateTimeFormatAttribute>().SingleOrDefault();
            if (dateTimeFormatAttribute != null)
            {
                value = ((DateTime)value).ToString(dateTimeFormatAttribute.Format);
                return value;
            }

            var toStringMethod = typeof(DateTime).GetMethod("ToString", new[] { typeof(CultureInfo) });
            value = toStringMethod.Invoke(value, new object[] { CultureInfo.InvariantCulture });
            return value;
        }
    }
}
