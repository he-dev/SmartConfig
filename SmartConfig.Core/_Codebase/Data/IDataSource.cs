using System;
using System.Collections.Generic;
using SmartConfig.Collections;

namespace SmartConfig.Data
{
    /// <summary>
    /// Delegate for defining filtering methods.
    /// </summary>
    /// <param name="elements">Elements to be filtered.</param>
    /// <param name="criteria">FilterSettings criteria as key & value.</param>
    /// <returns>Filtered elements.</returns>
    public delegate IEnumerable<IIndexer> FilterByFunc(IEnumerable<IIndexer> elements, KeyValuePair<string, string> criteria);

    /// <summary>
    /// Specifies the data source interface.
    /// </summary>
    public interface IDataSource
    {
        IReadOnlyCollection<Type> SupportedTypes { get; }

        /// <summary>
        /// Selects data from the data source.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        object Select(SettingKeyReadOnlyCollection keys);

        /// <summary>
        /// Updates data in the data source.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="value"></param>
        void Update(SettingKeyReadOnlyCollection keys, object value);
    }
}
