using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    /// <summary>
    /// Provides methods for selecting and updating data.
    /// </summary>
    public abstract class DataSourceBase
    {
        protected DataSourceBase()
        {
            FilterByEnvironment = (elements, s) => elements;
            FilterByVersion = (elements, s) => elements;
        }

        public Func<IEnumerable<ConfigElement>, string, IEnumerable<ConfigElement>> FilterByEnvironment { get; set; }

        public Func<IEnumerable<ConfigElement>, string, IEnumerable<ConfigElement>> FilterByVersion { get; set; }

        public abstract IEnumerable<ConfigElement> Select(string environment, string version, string name);

        public abstract void Update(ConfigElement configElement);
    }
}
