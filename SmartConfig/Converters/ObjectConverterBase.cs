using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
        protected ObjectConverterBase()
        {
            SupportedTypes = new HashSet<Type>();
        }

        protected ObjectConverterBase(IEnumerable<Type> supportedTypes)
        {
            SupportedTypes = new HashSet<Type>(supportedTypes);
        }

        /// <summary>
        /// Gets field types directly supported by the converter. 
        /// If left empty the conveter type will by used and you need to set the <c>ObjectConverterAttributes</c> on your field(s).
        /// </summary>
        public HashSet<Type> SupportedTypes { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this converter can directly convert field values.
        /// </summary>
        internal bool IsDirectConverter
        {
            get { return SupportedTypes.Count > 0; }
        }

        public bool SupportsAllTypes
        {
            get { return SupportedTypes.Count == 0; }
        }

        protected void ValidateType(Type type)
        {
            if (type.IsNullable())
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (type.IsEnum)
            {
                type = typeof(Enum);
            }

            var isSupportedType = SupportsAllTypes || SupportedTypes.Contains(type);
            if (!isSupportedType)
            {
                throw new UnsupportedTypeException(type);
            }
        }

        /// <summary>
        /// Deserializes the value to the specified type.
        /// </summary>
        /// <param name="value">Value to be deserialized.</param>
        /// <param name="type">Type of the value.</param>
        /// <param name="constraints"></param>
        /// <returns></returns>
        public abstract object DeserializeObject(string value, Type type, IEnumerable<ValueContraintAttribute> constraints);

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="value">Value to be serialized.</param>
        /// <param name="type"></param>
        /// <param name="constraints"></param>
        /// <returns></returns>
        public abstract string SerializeObject(object value, Type type, IEnumerable<ValueContraintAttribute> constraints);
    }
}
