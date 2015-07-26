using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    public class ColorConverter : ObjectConverterBase
    {
        public ColorConverter()
            : base(new[] { typeof(Color) })
        {
        }

        public override object DeserializeObject(string value, Type type, IEnumerable<ValueContraintAttribute> constraints)
        {
            ValidateType(type);
            return Color32.Parse(value);
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ValueContraintAttribute> constraints)
        {
            ValidateType(type);
            if (value != null) return value.ToString();
            if (!constraints.AllowNull()) throw new ArgumentNullException("value");
            return null;
        }
    }
}
