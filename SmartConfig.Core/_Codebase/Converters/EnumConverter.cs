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

        public override object DeserializeObject(string value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);

            // nullable types are not supported
            //if (type.IsNullable())
            //{
            //    if (string.IsNullOrEmpty(value))
            //    {
            //        // It is ok to return null for nullable types.
            //        return null;
            //    }
            //    // Otherwise get the underlying type.
            //    type = Nullable.GetUnderlyingType(type);
            //}           

            var result = Enum.Parse(type, value);
            return result;
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);         

            var result = value.ToString();
            return result;
        }
    }
}
