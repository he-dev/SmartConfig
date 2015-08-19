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


        protected DataSource()
        {
            KeyProperties = new Dictionary<string, KeyProperties>();
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

        public IDictionary<string, KeyProperties> KeyProperties { get; set; }

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
                .Aggregate(elements, (current, item) => KeyProperties[item.Key].Filter(current, item).Cast<TSetting>());
            return elements;
        }

        protected CompositeKey CreateCompositeKey(string defaultKey)
        {
            var complexKey = new CompositeKey()
            {
                { KeyNames.DefaultKeyName, defaultKey }
            };

            foreach (var keyName in KeyMembers.Where(k => k != KeyNames.DefaultKeyName))
            {
                complexKey[keyName] = KeyProperties[keyName].Value;
            }

            return complexKey;
        }
    }
}
