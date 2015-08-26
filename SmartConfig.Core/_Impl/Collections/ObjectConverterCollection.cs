using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Converters;

namespace SmartConfig.Collections
{
    /// <summary>
    /// Stores object converters.
    /// </summary>
    public class ObjectConverterCollection : IEnumerable<ObjectConverter>
    {
        private readonly Dictionary<Type, ObjectConverter> _converters = new Dictionary<Type, ObjectConverter>();

        /// <summary>
        /// Gets an object converter for the specified type or null.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ObjectConverter this[Type type]
        {
            get
            {
                ObjectConverter objectConverter;
                return _converters.TryGetValue(type, out objectConverter) ? objectConverter : null;
            }
        }

        /// <summary>
        /// Adds an object converter to the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectConverter"></param>
        public void Add<T>(T objectConverter) where T : ObjectConverter
        {
            if (objectConverter.IsDirectConverter)
            {
                foreach (var fieldType in objectConverter.SupportedTypes)
                {
                    _converters[fieldType] = objectConverter;
                }
            }
            else
            {
                _converters[typeof(T)] = objectConverter;
            }
        }

        public IEnumerator<ObjectConverter> GetEnumerator()
        {
            return _converters.Values.Distinct().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
