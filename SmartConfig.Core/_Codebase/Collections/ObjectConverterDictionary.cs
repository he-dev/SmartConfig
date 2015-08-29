using System;
using System.Collections.Generic;
using SmartConfig.Converters;

namespace SmartConfig.Collections
{
    /// <summary>
    /// Maps types to their supporting converters.
    /// </summary>
    public sealed class ObjectConverterDictionary : Dictionary<Type, ObjectConverter>
    {
        // don't let create this dictionary outside the assembly
        internal ObjectConverterDictionary() { }

        /// <summary>
        /// Gets an object converter for the specified type or null.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        new public ObjectConverter this[Type type]
        {
            get
            {
                ObjectConverter objectConverter;
                return TryGetValue(type, out objectConverter) ? objectConverter : null;
            }
            private set { ((Dictionary<Type, ObjectConverter>)this)[type] = value; }
        }

        public ObjectConverter this[SettingInfo settingInfo]
        {
            get
            {
                var objectConverter = this[settingInfo.ConverterType];
                if (objectConverter == null)
                {
                    throw new ObjectConverterNotFoundException(settingInfo);
                }
                return objectConverter;
            }
            set { this[settingInfo.ConfigType] = value; }
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
                this[supportedType] = objectConverter;
            }
        }
    }
}
