using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Tests
{
    public class TestDataSource : IDataSource
    {
        public Func<IDictionary<string, string>, string> SelectFunc;

        public Action<IDictionary<string, string>, string> UpdateAction;

        public string Select(IDictionary<string, string> keys)
        {
            return SelectFunc(keys);
        }

        public void Update(IDictionary<string, string> keys, string value)
        {
            UpdateAction(keys, value);
        }
    }
}
