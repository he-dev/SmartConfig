using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements and extends the <c>IDataSource</c> interface.
    /// </summary>
    public abstract class DataSource<TSetting> : IDataSource where TSetting : Setting, new()
    {
        private KeyNames _keyNames;

        private IDictionary<string, CustomKey> _customKeys = new Dictionary<string, CustomKey>();

        public KeyNames KeyNames => _keyNames ?? (_keyNames = KeyNames.From<TSetting>());

        protected IEnumerable<string> KeyNamesWithoutDefault
        {
            get { return KeyNames.Where(k => k != KeyNames.DefaultKeyName); }
        }

        public CustomKey[] CustomKeys
        {
            get { return _customKeys.Values.ToArray(); }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(CustomKeys));
                _customKeys = value.ToDictionary(x => x.Name);
            }
        }

        public bool SettingsInitializationEnabled { get; set; }

        public abstract string Select(string defaultKeyValue);

        public abstract void Update(string defaultKeyValue, string value);

        /// <summary>
        /// Applies all of the specified filters.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="compositeKey"></param>
        /// <returns></returns>
        protected IEnumerable<TSetting> ApplyFilters(IEnumerable<TSetting> elements, CompositeKey compositeKey)
        {
            elements = compositeKey
                .Where(x => x.Key != KeyNames.DefaultKeyName)
                .Aggregate(elements, (current, item) => _customKeys[item.Key].Filter(current, item).Cast<TSetting>());
            return elements;
        }

        protected CompositeKey CreateCompositeKey(string defaultKeyValue)
        {
            return new CompositeKey(defaultKeyValue, KeyNames, _customKeys);
        }
    }
}
