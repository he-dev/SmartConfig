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

        public override object DeserializeObject(string value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);

            var parseMethod = type.GetMethod("Parse", new[] { typeof(string) });
            var result = parseMethod.Invoke(null, new object[] { value });
            return result;
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);

            var toStringMethod = type.GetMethod("ToString", new Type[] { });
            var result = toStringMethod.Invoke(value, null);
            return (string)result;
        }
    }
}
