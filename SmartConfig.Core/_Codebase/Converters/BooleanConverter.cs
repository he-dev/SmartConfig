using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using SmartUtilities;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts value types from and to a string.
    /// </summary>
    public class BooleanConverter : ObjectConverter
    {
        public BooleanConverter() : base(new[] { typeof(bool) }) { }

        public override object DeserializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (HasTargetType(value, type)) { return value; }

            try
            {
                var parseMethod = type.GetMethod("Parse", new[] { typeof(string) });
                var result = parseMethod.Invoke(null, new[] { value });
                return result;
            }
            catch (TargetInvocationException inner)
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

            try
            {
                var toStringMethod = typeof(bool).GetMethod("ToString", new Type[] { });
                var result = toStringMethod.Invoke(value, null);
                return (string)result;
            }
            catch (TargetException inner)
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
