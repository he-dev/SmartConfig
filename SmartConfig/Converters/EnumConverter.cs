using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    public class EnumConverter : ObjectConverterBase
    {
        public EnumConverter()
            : base(new[] { typeof(Enum) })
        {
        }

        public override object DeserializeObject(string value, Type type, IEnumerable<ValueContraintAttribute> constraints)
        {
            ValidateType(type);

            if (type.IsNullable())
            {
                if (string.IsNullOrEmpty(value))
                {
                    // It is ok to return null for nullable types.
                    return null;
                }
                // Otherwise get the underlying type.
                type = Nullable.GetUnderlyingType(type);
            }

            var result = Enum.Parse(type, value);
            return result;
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ValueContraintAttribute> constraints)
        {
            ValidateType(type);

            if (value == null)
            {
                // It is ok to return null for null objects.
                return null;
            }

            var result = value.ToString();
            return result;
        }
    }
}
