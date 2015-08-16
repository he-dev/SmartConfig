using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements and extends the <c>IDataSource</c> interface.
    /// </summary>
    public abstract class DataSource<TConfigElement> : IDataSource where TConfigElement : ConfigElement, new()
    {
        protected IDictionary<string, KeyInfo<TConfigElement>> _keys;

        protected DataSource()
        {
            _keys = new Dictionary<string, KeyInfo<TConfigElement>>();
        }

        public IEnumerable<KeyInfo<TConfigElement>> Keys
        {
            get { return _keys.Values; }
            set
            {
                if (value == null) throw new ArgumentNullException("Keys");
                _keys = value.ToDictionary(k => k.KeyName);
            }
        }

        IDictionary<string, string> IDataSource.Keys
        {
            get { return _keys.ToDictionary(x => x.Key, x => x.Value.KeyValue); }
        }

        public abstract string Select(IDictionary<string, string> keys);

        public abstract void Update(IDictionary<string, string> keys, string value);
    }
}
