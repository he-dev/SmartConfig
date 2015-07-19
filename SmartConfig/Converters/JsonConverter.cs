using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    public class JsonConverter : ObjectConverterBase
    {
        protected override bool CanConvert(Type type)
        {
            // It can convert everything.
            return true;
        }

        public override object DeserializeObject(string value, Type type)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
            return result;
        }

        public override string SerializeObject(object value)
        {
            if (value == null)
            {
                return null;
            }

            var result = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            return result;
        }
    }
}
