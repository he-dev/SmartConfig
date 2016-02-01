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

            try
            {
                return (Color)(new Color32((string)value));
            }
            catch (InvalidColorException inner)
            {
                throw SmartException.Create<DeserializationException>(ex =>
                {
                    ex.FromType = value.GetType().Name;
                    ex.ToType = type.Name;
                    ex.Value = value;
                }, inner);
            }
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
