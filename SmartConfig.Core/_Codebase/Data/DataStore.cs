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
            const BindingFlags customPropertiesBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            CustomKeyFilters =
                typeof(TSetting) == typeof(BasicSetting)
                // create an empty dictionary if the TSetting is not derived from Setting
                ? new Dictionary<string, IKeyFilter>()
                // get custom properties from the derived TSetting type and create filters
                : typeof(TSetting).GetProperties(customPropertiesBindingFlags).ToDictionary(x => x.Name, x =>
                {
                    var filterAttribute = x.GetCustomAttribute<KeyFilterAttribute>();
                    if (filterAttribute == null)
                    {
                        throw new FilterAttributeMissingException
                        {
                            DeclaringTypeName = typeof(TSetting).FullName,
                            PropertyName = x.Name,
                        };
                    }
                    var settingFilter = (IKeyFilter)Activator.CreateInstance(filterAttribute.FilterType);
                    return settingFilter;
                });
        }

        /// <summary>
        /// Gets filters for custom keys.
        /// </summary>
        internal IDictionary<string, IKeyFilter> CustomKeyFilters { get; }

        public abstract IReadOnlyCollection<Type> SupportedSettingValueTypes { get; }

        public bool NotifyChanged { get; set; }

        public event EventHandler<DataStoreChangedEventArgs> Changed = delegate { };

        public abstract object Select(SettingKeyCollection keys);

        public abstract void Update(SettingKeyCollection keys, object value);

        /// <summary>
        /// Applies all of the specified filters.
        /// </summary>
        protected IEnumerable<TSetting> ApplyFilters(IEnumerable<TSetting> settings, IEnumerable<SettingKey> customKeys)
        {
            settings =
                customKeys.Aggregate(
                    settings,
                    (current, key) => CustomKeyFilters[key.Name].Apply(current, key).Cast<TSetting>()
                );
            return settings;
        }
    }
}
