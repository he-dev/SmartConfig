using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartConfig.Converters
{
    public class XmlConverter : ObjectConverterBase
    {
        protected override bool CanConvert(Type type)
        {
            return type == typeof(XDocument) || type == typeof(XElement);
        }

        public override object DeserializeObject(string value, Type type)
        {
            if (!CanConvert(type))
            {
                throw new InvalidOperationException(string.Format("Type [{0}] is not supported.", type.Name));
            }

            var parseMethod = type.GetMethod("Parse", new Type[] { typeof(string) });
            var result = parseMethod.Invoke(null, new object[] { value });
            return result;
        }

        public override string SerializeObject(object value)
        {
            if (value == null)
            {
                return null;
            }

            var type = value.GetType();
            if (!CanConvert(type))
            {
                throw new InvalidOperationException(string.Format("Type [{0}] is not supported.", type.Name));
            }

            var result = value.ToString();
            return result;
        }
    }
}
