using System;
using System.Collections.Generic;
using System.Globalization;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts <c>DateTime</c> from and to a string. Without this attribute the invariant culture is used.
    /// </summary>
    public class DateTimeConverter : ObjectConverter
    {
        public DateTimeConverter() : base(new[] { typeof(DateTime) }) { }

        public override object DeserializeObject(object value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            if (value.GetType() == type) { return value; }

            constraints.Check<DateTimeFormatAttribute>(format =>
            {
                DateTime intermediateResult;
                if (!format.TryParseExact((string)value, out intermediateResult))
                {
                    throw new DateTimeFormatViolationException
                    {
                        Value = value.ToString(),
                        Format = format.Format
                    };
                }
                value = intermediateResult;
            });

            if (value.GetType() == type)
            {
                return value;
            }

            var result = DateTime.Parse((string)value, CultureInfo.InvariantCulture);
            return result;
        }

        public override object SerializeObject(object value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            if (value.GetType() == type) { return value; }

            object result = null;
            constraints.Check<DateTimeFormatAttribute>(format =>
            {
                result = ((DateTime)value).ToString(format.Format);
            });

            if (result != null) { return result; }

            var toStringMethod = type.GetMethod("ToString", new[] { typeof(CultureInfo) });
            result = toStringMethod.Invoke(value, new object[] { CultureInfo.InvariantCulture });
            return result;
        }
    }
}
