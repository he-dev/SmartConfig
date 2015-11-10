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
        /// <summary>
        /// Selects data from the data source.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        string Select(IReadOnlyCollection<SettingKey> keys);

        /// <summary>
        /// Updates data in the data source.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="value"></param>
        void Update(IReadOnlyCollection<SettingKey> keys, string value);
    }
}
