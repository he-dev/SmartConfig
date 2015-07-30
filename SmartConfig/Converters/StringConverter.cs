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

            constraints.Check<RegularExpressionAttribute>(regex =>
            {
                if (!regex.IsMatch(value)) throw new RegularExpressionException(value, regex);
            });

            return value;
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ValueConstraintAttribute> constraints)
        {
            ValidateType(type);

            constraints.Check<RegularExpressionAttribute>(pattern =>
            {
                if (!pattern.IsMatch((string)value)) throw new RegularExpressionException((string)value, pattern);
            });

            return (string)value;
        }
    }
}
