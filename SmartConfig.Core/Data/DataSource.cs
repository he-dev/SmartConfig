using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements and extends the <c>IDataSource</c> interface.
    /// </summary>
    public abstract class DataSource<TSetting> : IDataSource where TSetting : Setting, new()
    {
        private KeyMembers _keyMembers;

        protected IDictionary<string, KeyProperties<TSetting>> _keyConfigurations;

        protected DataSource()
        {
            _keyConfigurations = new Dictionary<string, KeyProperties<TSetting>>();
        }        

        /// <summary>
        /// Gets an ordered enumeration of key names.
        /// The first element is always the Name then follow other key in alphabetical order.
        /// </summary>
        public KeyMembers KeyMembers
        {
            get
            {
                if (_keyMembers != null)
                {
                    return _keyMembers;
                }
                _keyMembers = KeyMembers.From<TSetting>();
                return _keyMembers;
            }
        }

        public bool CanInitializeSettings { get; set; }

        public virtual void InitializeSettings(IDictionary<string, string> values) { }

        public abstract string Select(string defaultKey);

        public abstract void Update(string defaultKey, string value);

        /// <summary>
        /// Applies all of the specified filters.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected IEnumerable<TSetting> ApplyFilters(IEnumerable<TSetting> elements, IDictionary<string, string> keys)
        {
            elements = keys
                .Where(x => x.Key != KeyNames.DefaultKeyName)
                .Aggregate(elements, (current, item) => _keyConfigurations[item.Key].Filter(current, item));
            return elements;
        }

        public DataSource<TSetting> ConfigureKey(string keyName, Action<KeyProperties<TSetting>> configureKey)
        {
            var keyConfiguration = new KeyProperties<TSetting>();
            configureKey(keyConfiguration);
            _keyConfigurations[keyName] = keyConfiguration;
            return this;
        }

        protected CompositeKey CreateCompositeKey(string defaultKey)
        {
            var complexKey = new CompositeKey()
            {
                { KeyNames.DefaultKeyName, defaultKey }
            };

            foreach (var keyName in KeyMembers.Where(k => k != KeyNames.DefaultKeyName))
            {
                complexKey[keyName] = _keyConfigurations[keyName].Value;
            }

            return complexKey;
        }
    }
}
