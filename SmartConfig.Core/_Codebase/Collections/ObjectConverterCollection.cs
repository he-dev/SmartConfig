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
        private readonly Dictionary<Type, ObjectConverter> _typeConverters = new Dictionary<Type, ObjectConverter>();

        // don't let create this dictionary outside the assembly
        internal ObjectConverterCollection() { }

        /// <summary>
        /// Gets an object converter for the specified type or an empty converter.
        /// </summary>
        /// <param name="converterType"></param>
        /// <returns></returns>
        public ObjectConverter this[Type converterType]
        {
            get
            {
                ObjectConverter objectConverter;
                if (!_typeConverters.TryGetValue(converterType, out objectConverter))
                {
                    throw new ConventerNotFoundException
                    {
                        SettingTypeFullName = converterType.FullName
                    };
                }
                return objectConverter;
                //: null;// _typeConverters[typeof(EmptyConverter)];
            }
        }

        public int TypeCount => _typeConverters.Count;

        /// <summary>
        /// Adds an object converter to the dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectConverter"></param>
        public void Add<T>(T objectConverter) where T : ObjectConverter
        {
            foreach (var supportedType in objectConverter.SupportedTypes)
            {
                //if (_typeConverters.ContainsKey(supportedType))
                //{
                //    throw new DuplicateTypeConverterException
                //    {
                //        ConverterFullName = objectConverter.GetType().FullName,
                //        TypeName = supportedType.Name
                //    };
                //}
                _typeConverters[supportedType] = objectConverter;
            }
        }

        public IEnumerator<ObjectConverter> GetEnumerator()
        {
            return _typeConverters.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
