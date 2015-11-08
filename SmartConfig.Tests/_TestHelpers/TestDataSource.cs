using System;
using System.Linq;
using SmartConfig.Data;

namespace SmartConfig.Tests
{
    public class SimpleTestDataSource : DataSource<Setting>
    {
        public SimpleTestDataSource() : base(null) { }

        public Func<string, string> SelectFunc;

        public Action<string, string> UpdateAction;

        public override string Select(string defaultKeyValue)
        {
            return SelectFunc(defaultKeyValue);
        }

        public override void Update(string defaultKeyValue, string value)
        {
            UpdateAction(defaultKeyValue, value);
        }
    }

    public class TestDataSource : DataSource<TestSetting>
    {
        public TestDataSource() : base(Enumerable.Empty<CustomKey>()) { }

        public Func<string, string> SelectFunc;

        public Action<string, string> UpdateAction;

        public override string Select(string defaultKeyValue)
        {
            return SelectFunc(defaultKeyValue);
        }

        public override void Update(string defaultKeyValue, string value)
        {
            UpdateAction(defaultKeyValue, value);
        }
    }
}
