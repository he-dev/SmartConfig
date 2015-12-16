using System;
using System.Collections.Generic;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Does nothing but returning the same value. It requires however that the value type matches the target type or othewise the <exception cref="ValueTypeMismatchException">ValueTypeMismatchException</exception> is thrown.
    /// </summary>
    public class EmptyConverter : ObjectConverter
    {
        public override object DeserializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (value.GetType() != type)
            {
                throw new ValueTypeMismatchException
                {
                    ValueTypeName = value.GetType().Name,
                    TargetTypeName = type.Name
                };
            }
            return value;
        }

        public override object SerializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (value.GetType() != type)
            {
                throw new ValueTypeMismatchException
                {
                    ValueTypeName = value.GetType().Name,
                    TargetTypeName = type.Name
                };
            }
            return value;
        }
    }
}
