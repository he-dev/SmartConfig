using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts XML from and to a string.
    /// </summary>
    public class XmlConverter : ObjectConverter
    {
        public XmlConverter()
            : base(new[]
            {
                typeof(XDocument),
                typeof(XElement)
            })
        {
        }

        public override object DeserializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (value.GetType() == type) { return value; }

            var parseMethod = type.GetMethod("Parse", new[] { typeof(string) });
            var result = parseMethod.Invoke(null, new object[] { value });
            return result;
        }

        public override object SerializeObject(object value, Type type, IEnumerable<Attribute> attributes)
        {
            if (value.GetType() == type) { return value; }

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            {
                var saveMethod = type.GetMethod("Save", new[] { typeof(StreamWriter), typeof(SaveOptions) });
                saveMethod.Invoke(value, new object[] { streamWriter, SaveOptions.DisableFormatting });
                var xml = Encoding.UTF8.GetString(memoryStream.ToArray());
                return xml;
            }

        }
    }
}
