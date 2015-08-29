using System.Collections.Generic;

namespace SmartConfig.Data
{
    /// <summary>
    /// Delegate for defining filtering methods.
    /// </summary>
    /// <param name="elements">Elements to be filtered.</param>
    /// <param name="criteria">Filter criteria as key & value.</param>
    /// <returns>Filtered elements.</returns>
    public delegate IEnumerable<IIndexer> FilterByFunc(IEnumerable<IIndexer> elements, KeyValuePair<string, string> criteria);

    /// <summary>
    /// Specifies the data source interface.
    /// </summary>
    public interface IDataSource
    {
        KeyNames KeyNames { get; }

        bool SettingsInitializationEnabled { get; }

        /// <summary>
        /// Selects data from the data source.
        /// </summary>
        /// <param name="defaultKeyValue"></param>
        /// <returns></returns>
        string Select(string defaultKeyValue);

        /// <summary>
        /// Updates data in the data source.
        /// </summary>
        /// <param name="defaultKeyValue"></param>
        /// <param name="value"></param>
        void Update(string defaultKeyValue, string value);
    }
}
