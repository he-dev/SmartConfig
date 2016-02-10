using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SmartConfig.Converters
{
    public class DateTimeConverter : ObjectConverter
    {
        private static readonly string[] DateTimeFormats = new string[]
        {
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-M-d H:m:s",
        };

        internal static string DefaultDateTimeFormat => DateTimeFormats[0];

        public DateTimeConverter() : base(new[] { typeof(DateTime) }) { }

        public override object DeserializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (HasTargetType(value, type)) { return value; }

            DateTime result;

            var dateTimeFormatAttribute = attributes.OfType<DateTimeFormatAttribute>().SingleOrDefault();
            if (dateTimeFormatAttribute != null)
            {
                if (!DateTime.TryParseExact((string)value, dateTimeFormatAttribute.Format, null, DateTimeStyles.None, out result))
                {
                    throw new InvalidValueException
                    {
                        Value = value.ToString(),
                        ExpectedFormat = dateTimeFormatAttribute.Format
                    };
                }
                return result;
            }

            if (!DateTime.TryParseExact((string)value, DateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                throw new InvalidValueException
                {
                    Value = value.ToString(),
                    ExpectedFormat = string.Join(", ", DateTimeFormats)
                };
            }

            return result;
        }

        public override object SerializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (HasTargetType(value, type)) { return value; }

            CheckValueType(value);

            var dateTime = ((DateTime)value);

            var dateTimeFormatAttribute = attributes.OfType<DateTimeFormatAttribute>().SingleOrDefault();
            if (dateTimeFormatAttribute != null)
            {
                value = dateTime.ToString(dateTimeFormatAttribute.Format);
                return value;
            }

            value = dateTime.ToString(DefaultDateTimeFormat);
            return value;
        }
    }
}
