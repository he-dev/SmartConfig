using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts JSON data from and to a string.
    /// </summary>
    public class JsonConverter : ObjectConverter
    {
        public override object DeserializeObject(object value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            if (value.GetType() == type) { return value; }

            var result = JsonConvert.DeserializeObject((string)value, type);
            return result;
        }

        public override object SerializeObject(object value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            if (value.GetType() == type) { return value; }

            var result = JsonConvert.SerializeObject(value);
            return result;
        }
    }
}
