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
            if (HasTargetType(value, type)) { return value; }

            int result;
            if (!ColorParser.TryParse((string)value, out result))
            {
                throw new InvalidValueException
                {
                    Value = value.ToString(),
                    ExpectedFormat = "Known color name (White), Hex (#FFFFFF), Decimal (255, 255, 255)"
                };
            }

            return (Color)(new Color32(result));
        }

        public override object SerializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (HasTargetType(value, type)) { return value; }

            CheckValueType(value);

            try
            {
                return ((Color32)(Color)value).ToString();
            }
            catch (InvalidColorException inner)
            {
                throw SmartException.Create<SerializationException>(ex =>
                {
                    ex.FromType = value.GetType().Name;
                    ex.ToType = type.Name;
                    ex.Value = value;
                }, inner);
            }
        }
    }
}
