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

        public override object DeserializeObject(string value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);

            DateTime? customResult = null;
            constraints.Check<DateTimeFormatAttribute>(format =>
            {
                DateTime intermediateResult;
                if (!format.TryParseExact(value, out intermediateResult))
                {
                    throw new DateTimeFormatViolationException
                    {
                        Value = value,
                        Format = format.Format
                    };
                }
                customResult = intermediateResult;
            });

            if (customResult.HasValue)
            {
                return customResult.Value;
            }

            var result = DateTime.Parse(value, CultureInfo.InvariantCulture);
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
                var toStringMethod = type.GetMethod("ToString", new[] { typeof(CultureInfo) });
                result = (string)toStringMethod.Invoke(value, new object[] { CultureInfo.InvariantCulture });
            }
            return result;
        }
    }
}
