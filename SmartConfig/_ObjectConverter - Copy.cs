using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace SmartConfig
{
    /// <summary>
    /// Encapsulates a method that deserializes the data.
    /// </summary>
    /// <param name="data">Data to be deserialized.</param>
    /// <param name="type">Type of the data to deserialize to.</param>
    /// <returns></returns>
    public delegate object DeserializeObjectFunc(string data, Type type);

    /// <summary>
    /// Encapsulates a method that serializes an object.
    /// </summary>
    /// <param name="obj">Object to be serialized.</param>
    /// <returns></returns>
    public delegate string SerializeObjectFunc(object obj);

    /// <summary>
    /// Provides (de)serialization of various types.
    /// </summary>
    public abstract class ObjectConverter
    {        
        /// <summary>
        /// Defines all available <c>ObjectConverter</c>s.
        /// </summary>
        public static readonly Dictionary<Type, ObjectConverter> ObjectConverters = new Dictionary<Type, ObjectConverter>();

        /// <summary>
        /// Constructs the <c>ObjectConverter</c>.
        /// </summary>
        static ObjectConverter()
        {
            InitializeObjectConverters();
        }

        public Type Type { get; protected set; }

        /// <summary>
        /// Allows to specify the deserialization method.
        /// </summary>
        public DeserializeObjectFunc DeserializeObjectFunc { get; set; }

        /// <summary>
        /// Allows to specify the serialization method.
        /// </summary>
        public SerializeObjectFunc SerializeObjectFunc { get; set; }

        public abstract object DeserializeObject(string data);

        public abstract string SerializeObject(object obj);

        private static void InitializeObjectConverters()
        {
            #region char
            ObjectConverters[typeof(char)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => char.Parse(value),
                SerializeObjectFunc = (value) => ((char)value).ToString(CultureInfo.InvariantCulture),
            };
            ObjectConverters[typeof(char?)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => value.DeserializeObjectOrNull(type, (v, t) => char.Parse(v)),
                SerializeObjectFunc = (value) => value.SerializeObjectOrNull((v) => ((char)v).ToString(CultureInfo.InvariantCulture)),
            };
            #endregion

            #region string
            // Dummy string converter to keep the converters ...
            ObjectConverters[typeof(string)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => value,
                SerializeObjectFunc = (value) => value.ToString()
            };
            #endregion

            #region bool
            ObjectConverters[typeof(bool)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => bool.Parse(value),
                SerializeObjectFunc = (value) => value.ToString(),
            };

            ObjectConverters[typeof(bool?)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => value.DeserializeObjectOrNull(type, (v, t) => bool.Parse(v)),
                SerializeObjectFunc = (value) => value.SerializeObjectOrNull((v) => ((bool)v).ToString(CultureInfo.InvariantCulture)),
            };
            #endregion

            #region short
            ObjectConverters[typeof(short)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => short.Parse(value, CultureInfo.InvariantCulture),
                SerializeObjectFunc = (value) => ((short)value).ToString(CultureInfo.InvariantCulture),
            };
            ObjectConverters[typeof(short?)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => value.DeserializeObjectOrNull(type, (v, t) => short.Parse(v, CultureInfo.InvariantCulture)),
                SerializeObjectFunc = (value) => value.SerializeObjectOrNull((v) => ((short)v).ToString(CultureInfo.InvariantCulture)),
            };
            #endregion

            #region int
            ObjectConverters[typeof(int)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => int.Parse(value, CultureInfo.InvariantCulture),
                SerializeObjectFunc = (value) => ((int)value).ToString(CultureInfo.InvariantCulture),
            };

            ObjectConverters[typeof(int?)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => value.DeserializeObjectOrNull(type, (v, t) => int.Parse(v, CultureInfo.InvariantCulture)),
                SerializeObjectFunc = (value) => value.SerializeObjectOrNull((v) => ((int)v).ToString(CultureInfo.InvariantCulture)),
            };
            #endregion

            #region long
            ObjectConverters[typeof(long)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => long.Parse(value, CultureInfo.InvariantCulture),
                SerializeObjectFunc = (value) => ((long)value).ToString(CultureInfo.InvariantCulture),
            };

            ObjectConverters[typeof(long?)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => value.DeserializeObjectOrNull(type, (v, t) => long.Parse(v, CultureInfo.InvariantCulture)),
                SerializeObjectFunc = (value) => value.SerializeObjectOrNull((v) => ((long)v).ToString(CultureInfo.InvariantCulture)),
            };
            #endregion

            #region single
            ObjectConverters[typeof(float)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => float.Parse(value, CultureInfo.InvariantCulture),
                SerializeObjectFunc = (value) => ((float)value).ToString(CultureInfo.InvariantCulture),
            };

            ObjectConverters[typeof(float?)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => value.DeserializeObjectOrNull(type, (v, t) => float.Parse(v, CultureInfo.InvariantCulture)),
                SerializeObjectFunc = (value) => value.SerializeObjectOrNull(v => ((float)v).ToString(CultureInfo.InvariantCulture)),
            };
            #endregion

            #region double
            ObjectConverters[typeof(double)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => double.Parse(value, CultureInfo.InvariantCulture),
                SerializeObjectFunc = (value) => ((double)value).ToString(CultureInfo.InvariantCulture),
            };

            ObjectConverters[typeof(double?)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => value.DeserializeObjectOrNull(type, (v, t) => double.Parse(v, CultureInfo.InvariantCulture)),
                SerializeObjectFunc = (value) => value.SerializeObjectOrNull(v => ((double)v).ToString(CultureInfo.InvariantCulture)),
            };
            #endregion

            #region decimal
            ObjectConverters[typeof(decimal)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => decimal.Parse(value, CultureInfo.InvariantCulture),
                SerializeObjectFunc = (value) => ((decimal)value).ToString(CultureInfo.InvariantCulture),
            };

            ObjectConverters[typeof(decimal?)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => value.DeserializeObjectOrNull(type, (v, t) => decimal.Parse(v, CultureInfo.InvariantCulture)),
                SerializeObjectFunc = (value) => value.SerializeObjectOrNull(v => ((decimal)v).ToString(CultureInfo.InvariantCulture)),
            };
            #endregion

            #region Json
            ObjectConverters[typeof(JsonConverter)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => value.DeserializeObjectOrNull(type, (v, t) => Newtonsoft.Json.JsonConvert.DeserializeObject(v, t)),
                SerializeObjectFunc = (value) => value.SerializeObjectOrNull(v => Newtonsoft.Json.JsonConvert.SerializeObject(v)),
            };
            #endregion

            #region XDocument
            ObjectConverters[typeof(XDocument)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => value.DeserializeObjectOrNull(type, (v, t) => XDocument.Parse(v)),
                SerializeObjectFunc = (value) => value.SerializeObjectOrNull(v => v.ToString()),
            };
            #endregion

            #region XElement
            ObjectConverters[typeof(XElement)] = new ObjectConverter()
            {
                DeserializeObjectFunc = (value, type) => value.DeserializeObjectOrNull(type, (v, t) => XElement.Parse(v)),
                SerializeObjectFunc = (value) => value.SerializeObjectOrNull(v => v.ToString()),
            };
            #endregion
        }

        internal static object DeserializeObject(string value, Type type, Type objectConverterType)
        {
            return GetObjectConverter(type, objectConverterType).DeserializeObjectFunc(value, type);
        }

        internal static string SerializeObject<T>(T value, Type objectConverterType)
        {
            return GetObjectConverter(typeof(T), objectConverterType).SerializeObjectFunc(value);
        }

        internal static ObjectConverter GetObjectConverter(Type type, Type objectConverterType)
        {
            ObjectConverter objectConverter;
            if (!ObjectConverters.TryGetValue(objectConverterType ?? type, out objectConverter))
            {
                throw new KeyNotFoundException(string.Format("Object converter for [{0}] not found.", type.Name));
            }
            return objectConverter;
        }
    }
}
