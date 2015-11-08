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

        private IDictionary<string, CustomKey> _customKeys;

        protected DataSource(IEnumerable<CustomKey> customKeys)
        {
            _customKeys = new Dictionary<string, CustomKey>();

            var isDerived = typeof(TSetting) != typeof(Setting);
            if (isDerived)
            {
                if (customKeys == null)
                {
                    throw new ArgumentNullException(nameof(customKeys));
                }

                var customKeyNames = _keyNames.Where(kn => kn != KeyNames.DefaultKeyName).ToList();
                var allKeysConfigured = customKeys.All(ck => ck.Filter != null && customKeyNames.Contains(ck.Name));
                if (!allKeysConfigured)
                {
                    // todo throw custom exception here
                    throw new ArgumentException(null, nameof(customKeys));
                }

                _customKeys = customKeys.ToDictionary(ck => ck.Name);
            }

        }

        public KeyNames KeyNames => _keyNames ?? (_keyNames = KeyNames.From<TSetting>());

        protected IEnumerable<string> KeyNamesWithoutDefault
        {
            get { return KeyNames.Where(k => k != KeyNames.DefaultKeyName); }
        }

        public CustomKey[] CustomKeys => _customKeys.Values.ToArray();

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
