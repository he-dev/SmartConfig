using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SmartConfig.Collections;
using SmartConfig.DataAnnotations;
using SmartConfig.Filters;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements and extends the <c>IDataStore</c> interface.
    /// </summary>
    public abstract class DataStore<TSetting> : IDataStore where TSetting : BasicSetting, new()
    {
        private readonly IDictionary<string, object> _customKeyValues;

        protected DataStore()
        {
            var setting = new TSetting();
            SettingType = typeof(TSetting);
            _customKeyValues = setting.CustomKeyProperties.ToDictionary(p => p.Name, p => (object)null);
            CustomKeyFilters = setting.GetCustomSettingFilters();
        }

        public Type SettingType { get; }

        public IReadOnlyDictionary<string, object> CustomKeyValues => new ReadOnlyDictionary<string, object>(_customKeyValues);

        //=> new ReadOnlyDictionary<string, object>(_customKeyValues);

        public IReadOnlyDictionary<string, ISettingFilter> CustomKeyFilters { get; }

        public abstract IReadOnlyCollection<Type> SerializationDataTypes { get; }

        public Type DefaultSerializationDataType => SerializationDataTypes.FirstOrDefault();

        public Type GetSerializationDataType(Type objectType)
        {
            return SerializationDataTypes.Contains(objectType) ? objectType : DefaultSerializationDataType;
        }

        public void SetCustomKey(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }
            if (value == null) { throw new ArgumentNullException(nameof(value)); }

            if (!_customKeyValues.ContainsKey(name))
            {
            }
            _customKeyValues[name] = value;
        }

        public abstract object Select(SettingKey key);

        public abstract void Update(SettingKey key, object value);

        /// <summary>
        /// Applies all of the specified filters.
        /// </summary>
        protected IEnumerable<TSetting> ApplyFilters(IEnumerable<TSetting> settings, IDictionary<string, object> keys)
        {
            settings = keys.Aggregate
            (
                settings,
                (current, custom) => CustomKeyFilters[custom.Key].Apply(current, custom).Cast<TSetting>()
            );
            return settings;
        }
    }
}
