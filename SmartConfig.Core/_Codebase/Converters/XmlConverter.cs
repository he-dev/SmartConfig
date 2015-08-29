using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

        public override object DeserializeObject(string value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            ValidateType(type);
            var parseMethod = type.GetMethod("Parse", new Type[] { typeof(string) });
            var result = parseMethod.Invoke(null, new object[] { value });
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
