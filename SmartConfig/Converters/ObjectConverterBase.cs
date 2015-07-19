using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Converts objects to and from <c>string</c>.
    /// </summary>
    public abstract class ObjectConverterBase
    {
        protected abstract bool CanConvert(Type type);

        /// <summary>
        /// Deserializes the value to the specified type.
        /// </summary>
        /// <param name="value">Value to be deserialized.</param>
        /// <param name="type">Type of the value.</param>
        /// <returns></returns>
        public abstract object DeserializeObject(string value, Type type);

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="value">Value to be serialized.</param>
        /// <returns></returns>
        public abstract string SerializeObject(object value);      
    }
}
