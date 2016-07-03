using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartUtilities.TypeFramework;

namespace SmartConfig
{
    public class JsonToObjectConverter<T> : TypeConverter<String, T>
    {
        public override T Convert(string value, ConversionContext context)
        {
            return JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings { Culture = context.Culture });
        }
    }

    public class ObjectToJsonConverter<T> : TypeConverter<T, String>
    {
        public override string Convert(T value, ConversionContext context)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings { Culture = context.Culture });
        }
    }
}
