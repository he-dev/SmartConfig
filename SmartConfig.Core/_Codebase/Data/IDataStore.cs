using System;
using System.Collections.Generic;
using SmartConfig.Filters;

namespace SmartConfig.Data
{
    /// <summary>
    /// Delegate for defining filtering methods.
    /// </summary>
    /// <param name="elements">Elements to be filtered.</param>
    /// <param name="criteria">Apply criteria as key & value.</param>
    /// <returns>Filtered elements.</returns>
    public delegate IEnumerable<IIndexable> FilterByFunc(IEnumerable<IIndexable> elements, KeyValuePair<string, string> criteria);

    /// <summary>
    /// Specifies the data source interface.
    /// </summary>
    public interface IDataStore
    {
        Type SettingType { get; }

        IReadOnlyDictionary<string, object> CustomKeyValues { get; }

        IReadOnlyDictionary<string, ISettingFilter> CustomKeyFilters { get; }

        IReadOnlyCollection<Type> SerializationTypes { get; }

        Type DefaultSerializationType { get; }

        Type GetSerializationType(Type objectType);

        void SetCustomKey(string name, object value);

        /// <summary>
        /// Selects data from the data source.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Select(SettingKey key);

        /// <summary>
        /// Updates data in the data source.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Update(SettingKey key, object value);
    }
}
