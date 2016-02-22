using System;
using System.Collections.Generic;
using SmartConfig.Collections;

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
        IReadOnlyCollection<Type> SupportedSettingDataTypes { get; }

        //bool ChangedNotificationSupported { get; }

        //bool ChangedNotificationEnabled { get; set; }

        //event EventHandler<DataStoreChangedEventArgs> Changed;

        /// <summary>
        /// Selects data from the data source.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        object Select(CompoundSettingKey keys);

        /// <summary>
        /// Updates data in the data source.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="value"></param>
        void Update(CompoundSettingKey keys, object value);
    }
}
