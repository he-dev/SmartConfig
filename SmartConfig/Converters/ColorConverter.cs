using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    public class ColorConverter : ObjectConverterBase
    {
        protected override bool CanConvert(Type type)
        {
            return type == typeof(Color);
        }

        public override object DeserializeObject(string value, Type type)
        {
            if (!CanConvert(type))
            {
                throw new InvalidOperationException(string.Format("Type [{0}] is not supported.", type.Name));
            }

            return Color32.Parse(value);
        }

        public override string SerializeObject(object value)
        {
            if (value == null)
            {
                // It is ok to return null for null objects.
                return null;
            }

            var type = value.GetType();
            if (!CanConvert(type))
            {
                throw new InvalidOperationException(string.Format("Type [{0}] is not supported.", type.Name));
            }

            return value.ToString();
        }
    }
}
