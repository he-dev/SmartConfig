using System;
using System.Collections.Generic;
using System.Globalization;

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
            if (value.GetType() == type) { return value; }

            var parseMethod = type.GetMethod("Parse", new[] { typeof(string) });
            var result = parseMethod.Invoke(null, new[] { value });
            return result;
        }

        public override object SerializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (value.GetType() == type) { return value; }

            var toStringMethod = typeof(bool).GetMethod("ToString", new Type[] { });
            var result = toStringMethod.Invoke(value, null);
            return (string)result;
        }
    }
}
