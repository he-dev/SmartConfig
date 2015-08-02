using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public delegate IEnumerable<T> FilterByFunc<T>(IEnumerable<T> elements, KeyValuePair<string, string> keyValue) where T : class;   

    /// <summary>
    /// Specifies the data source interface.
    /// </summary>
    public interface IDataSource
    {
        string Select(IDictionary<string, string> keys);

        void Update(IDictionary<string, string> keys, string value);
    }
}
