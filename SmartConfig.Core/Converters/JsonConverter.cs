﻿using System;
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
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
            return result;
        }

        public override string SerializeObject(object value, Type type, IEnumerable<ValueConstraintAttribute> constraints)
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