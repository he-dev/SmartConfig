using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Dummy string converter.
    /// </summary>
    public class StringConverter : ObjectConverter
    {
        public StringConverter() : base(new[] { typeof(string) }) { }

        public override object DeserializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            Validate(value, attributes);
            return value;
        }

        public override object SerializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            Validate(value, attributes);
            return value;
        }
    }
}
