using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    public class StringConverter : ObjectConverterBase
    {
        public StringConverter() : base(new[] { typeof(string) }) { }

        public override object DeserializeObject(string value, Type type, IEnumerable<ValueContraintAttribute> constraints)
        {
            ValidateType(type);

            if (string.IsNullOrEmpty(value) && !constraints.AllowNull())
            {
                throw new ArgumentNullException("value", "Value must not be null.");
            }

            return value;
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ValueContraintAttribute> constraints)
        {
            ValidateType(type);

            if (string.IsNullOrEmpty((string)value) && !constraints.AllowNull())
            {
                throw new ArgumentNullException("value", "Value must not be null.");
            }

            return (string)value;
        }
    }
}
