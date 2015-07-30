using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts colors from and to string. Allowed values are known names, rgb and hex.
    /// </summary>
    public class ColorConverter : ObjectConverterBase
    {
        public ColorConverter()
            : base(new[] { typeof(Color) })
        {
        }

        public override object DeserializeObject(string value, Type type, IEnumerable<ValueConstraintAttribute> constraints)
        {
            ValidateType(type);
            return (Color)Color32.Parse(value);
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ValueConstraintAttribute> constraints)
        {
            ValidateType(type);
            return value != null ? value.ToString() : null;
        }
    }
}
