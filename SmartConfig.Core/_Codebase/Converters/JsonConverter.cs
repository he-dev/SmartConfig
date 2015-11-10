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
        public override object DeserializeObject(string value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            var result =
                !string.IsNullOrEmpty(value)
                ? JsonConvert.DeserializeObject(value, type)
                : null;
            return result;
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ConstraintAttribute> constraints)
        {
            var result =
                value != null
                ? JsonConvert.SerializeObject(value)
                : null;
            return result;
        }
    }
}
