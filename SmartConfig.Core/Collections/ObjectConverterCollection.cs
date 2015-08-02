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
    public class ObjectConverterCollection : IEnumerable<ObjectConverterBase>
    {
        private readonly Dictionary<Type, ObjectConverterBase> _converters = new Dictionary<Type, ObjectConverterBase>();

        /// <summary>
        /// Gets an object converter for the specified type or null.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ObjectConverterBase this[Type type]
        {
            get
            {
                ObjectConverterBase objectConverter;
                return _converters.TryGetValue(type, out objectConverter) ? objectConverter : null;
            }
        }

        /// <summary>
        /// Adds an object converter to the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectConverter"></param>
        public void Add<T>(T objectConverter) where T : ObjectConverterBase
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

        public IEnumerator<ObjectConverterBase> GetEnumerator()
        {
            return _converters.Values.Distinct().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
