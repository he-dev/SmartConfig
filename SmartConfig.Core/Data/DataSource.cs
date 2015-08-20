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
        private KeyNames _keyNames;

        protected DataSource()
        {
            KeyProperties = new Dictionary<string, KeyProperties>();
        }        

        /// <summary>
        /// Gets an ordered enumeration of key names.
        /// The first element is always the Name then follow other key in alphabetical order.
        /// </summary>
        public KeyNames KeyNames
        {
            get
            {
                if (_keyNames != null)
                {
                    return _keyNames;
                }
                _keyNames = KeyNames.From<TSetting>();
                return _keyNames;
            }
        }

        public IDictionary<string, KeyProperties> KeyProperties { get; set; }

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
                .Aggregate(elements, (current, item) => KeyProperties[item.Key].Filter(current, item).Cast<TSetting>());
            return elements;
        }        
    }
}
