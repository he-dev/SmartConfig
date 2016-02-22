using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartConfig.Collections;
using SmartConfig.Filters;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements and extends the <c>IDataStore</c> interface.
    /// </summary>
    public abstract class DataStore<TSetting> : IDataStore where TSetting : BasicSetting, new()
    {
        protected DataStore()
        {
            SettingFilters = SettingFilterDictionary.Create<TSetting>();
        }

        /// <summary>
        /// Gets filters for custom keys.
        /// </summary>
        internal SettingFilterDictionary SettingFilters { get; }

        public abstract IReadOnlyCollection<Type> SupportedSettingDataTypes { get; }

        //public bool ChangedNotificationSupported { get; protected set; } = false;

        //public virtual bool ChangedNotificationEnabled { get; set; } = false;

        //public event EventHandler<DataStoreChangedEventArgs> Changed = delegate { };

        public abstract object Select(CompoundSettingKey keys);

        public abstract void Update(CompoundSettingKey keys, object value);

        /// <summary>
        /// Applies all of the specified filters.
        /// </summary>
        protected IEnumerable<TSetting> ApplyFilters(IEnumerable<TSetting> settings, IEnumerable<SimpleSettingKey> customKeys)
        {
            settings =
                customKeys.Aggregate(
                    settings,
                    (current, key) => SettingFilters[key.Name].Apply(current, key).Cast<TSetting>()
                );
            return settings;
        }
    }
}
