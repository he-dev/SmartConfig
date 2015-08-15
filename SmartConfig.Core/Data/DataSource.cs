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
        private IDictionary<string, string> _keys;
        private IDictionary<string, FilterByFunc<TConfigElement>> _filters;

        protected DataSource()
        {
            _keys = new Dictionary<string, string>();
            _filters = new Dictionary<string, FilterByFunc<TConfigElement>>();
        }

        /// <summary>
        /// Allows to specify additional keys.
        /// </summary>
        public IDictionary<string, string> Keys
        {
            get { return _keys; }
            set
            {
                if (value == null) throw new ArgumentNullException("Keys");
                _keys = value;
            }
        }

        /// <summary>
        /// Allows to specifiy filter function for the additional keys.
        /// </summary>
        public IDictionary<string, FilterByFunc<TConfigElement>> Filters
        {
            get { return _filters; }
            set
            {
                if (value == null) throw new ArgumentNullException("Filters");
                _filters = value;
            }
        }

        public abstract string Select(IDictionary<string, string> keys);

        public abstract void Update(IDictionary<string, string> keys, string value);
    }
}
