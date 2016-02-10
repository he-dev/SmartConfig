using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using SmartUtilities;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts value types from and to a string.
    /// </summary>
    public class BooleanConverter : ObjectConverter
    {
        public BooleanConverter() : base(new[] { typeof(bool) }) { }

        public override object DeserializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (HasTargetType(value, type)) { return value; }

            //CheckValueType(value);

            bool result;
            if (!bool.TryParse((string)value, out result))
            {
                throw new InvalidValueException
                {
                    Value = value.ToString(),
                    ExpectedFormat = string.Join(", ", bool.TrueString, bool.FalseString)
                };
            }
            return result;
        }

        public override object SerializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (HasTargetType(value, type)) { return value; }

            CheckValueType(value);

            return ((bool)value).ToString();
        }
    }
}
