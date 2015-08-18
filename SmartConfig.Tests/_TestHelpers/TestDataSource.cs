using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Tests
{
    public class TestDataSource : DataSource<TestSetting>
    {
        public Func<string, string> SelectFunc;

        public Action<string, string> UpdateAction;

        public override void InitializeSettings(IDictionary<string, string> values)
        {
        }

        public override string Select(string defaultKey)
        {
            return SelectFunc(defaultKey);
        }

        public override void Update(string defaultKey, string value)
        {
            UpdateAction(defaultKey, value);
        }
    }
}
