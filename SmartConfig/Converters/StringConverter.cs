using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Dummy string converter.
    /// </summary>
    public class StringConverter : ObjectConverterBase
    {
        public StringConverter() : base(new[] { typeof(string) }) { }

        public override object DeserializeObject(string value, Type type, IEnumerable<ValueConstraintAttribute> constraints)
        {
            ValidateType(type);

            constraints.Check<NullableAttribute>(attribute =>
            {
                if (string.IsNullOrEmpty(value)) throw new NullableException(value);
            });

            constraints.Check<PatternAttribute>(pattern =>
            {
                if (!pattern.IsMatch(value)) throw new PatternException(value, pattern);
            });

            return value;
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ValueConstraintAttribute> constraints)
        {
            ValidateType(type);

            constraints.Check<NullableAttribute>(attribute =>
            {
                if (string.IsNullOrEmpty((string)value)) throw new NullableException(value);
            });

            constraints.Check<PatternAttribute>(pattern =>
            {
                if (!pattern.IsMatch((string)value)) throw new PatternException((string)value, pattern);
            });

            return (string)value;
        }
    }
}
