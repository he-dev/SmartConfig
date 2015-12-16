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
        public ColorConverter() : base(new[] { typeof(Color) }) { }

        public override object DeserializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (value.GetType() == type) { return value; }

            return (Color)(new Color32((string)value));
        }

        public override object SerializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (value.GetType() == type) { return value; }

            return value.ToString();
        }
    }
}
