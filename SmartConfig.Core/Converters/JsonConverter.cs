using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts JSON data from and to a string.
    /// </summary>
    public class JsonConverter : ObjectConverterBase
    {
        public override object DeserializeObject(string value, Type type, IEnumerable<ValueConstraintAttribute> constraints)
        {
            var result =
                !string.IsNullOrEmpty(value)
                ? Newtonsoft.Json.JsonConvert.DeserializeObject(value, type)
                : null;
            return result;
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ValueConstraintAttribute> constraints)
        {
            var result =
                value != null
                ? Newtonsoft.Json.JsonConvert.SerializeObject(value)
                : null;
            return result;
        }
    }
}
