using System;
using System.Collections.Generic;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts enums from and to a string.
    /// </summary>
    public class EnumConverter : ObjectConverter
    {
        public EnumConverter()
            : base(new[] { typeof(Enum) })
        {
        }

        public override object DeserializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (value.GetType() == type) { return value; }

            var result = Enum.Parse(type, (string)value);
            return result;
        }

        public override object SerializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (value.GetType() == type) { return value; }

            var result = value.ToString();
            return result;
        }
    }
}
