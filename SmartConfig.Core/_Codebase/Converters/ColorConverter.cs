using System;
using System.Collections.Generic;
using System.Drawing;
using SmartUtilities;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts colors from and to string. Allowed values are known names, rgb and hex.
    /// </summary>
    public class ColorConverter : ObjectConverter
    {
        public ColorConverter()
            : base(new[] { typeof(Color) })
        {
        }

        public override object DeserializeObject(string value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);
            return (Color)(new Color32(value));
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);
            return value.ToString();
        }
    }
}
