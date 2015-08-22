using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    /// <summary>
    /// Delegate for defining filtering methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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

        bool CanInitializeSettings { get; }

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
