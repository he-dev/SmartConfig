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
    public delegate IEnumerable<T> FilterByFunc<T>(IEnumerable<T> elements, KeyValuePair<string, string> criteria) where T : class;

    /// <summary>
    /// Specifies the data source interface.
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// Gets key used by the data source.
        /// </summary>
        IDictionary<string, string> CompositeKey { get; }

        IEnumerable<string> OrderedKeyNames { get; }

        bool InitializationEnabled { get; }

        void Initialize(IDictionary<string, string> values);

        /// <summary>
        /// Selects data from the data source.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string Select(string defaultKey);

        /// <summary>
        /// Updates data in the data source.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Update(string defaultKey, string value);

        CompositeKey CreateCompositeKey(string defaultKeyValue);
    }    
}
