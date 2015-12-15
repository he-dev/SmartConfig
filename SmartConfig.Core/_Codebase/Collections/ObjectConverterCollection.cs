using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SmartConfig.Converters;

namespace SmartConfig.Collections
{
    /// <summary>
    /// Maps types to their supporting converters.
    /// </summary>
    public sealed class ObjectConverterCollection : IObjectConverterCollection
    {
        private readonly Dictionary<Type, ObjectConverter> _converters = new Dictionary<Type, ObjectConverter>();

        // don't let create this dictionary outside the assembly
        internal ObjectConverterCollection() { }

        /// <summary>
        /// Gets an object converter for the specified type or null.
        /// </summary>
        /// <param name="converterType"></param>
        /// <returns></returns>
        public ObjectConverter this[Type converterType]
        {
            get
            {
                ObjectConverter objectConverter;
                if (!_converters.TryGetValue(converterType, out objectConverter))
                {
                    return _converters[typeof(EmptyConverter)];
                    //throw new ObjectConverterNotFoundException(converterType);
                }
                return objectConverter;
            }
            private set { _converters[converterType] = value; }
        }

        /// <summary>
        /// Adds an object converter to the dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectConverter"></param>
        public void Add<T>(T objectConverter) where T : ObjectConverter
        {
            foreach (var supportedType in objectConverter.SupportedTypes)
            {
                if (_converters.ContainsKey(supportedType))
                {
                    throw new DuplicateTypeConverterException
                    {
                        ConverterFullName = objectConverter.GetType().FullName,
                        TypeName = supportedType.Name
                    };
                }
                this[supportedType] = objectConverter;
            }
        }

        public IEnumerator<ObjectConverter> GetEnumerator()
        {
            return _converters.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
