using System;
using System.Collections.Generic;
using System.Linq;
using SmartConfig.Data;

namespace SmartConfig.Tests
{
    public class SimpleTestDataSource : DataSource<Setting>
    {
        public Func<IEnumerable<SettingKey>, string> SelectFunc;

        public Action<IEnumerable<SettingKey>, string> UpdateAction;

        public override string Select(IEnumerable<SettingKey> keys)
        {
            return SelectFunc(keys);
        }

        public override void Update(IEnumerable<SettingKey> keys, string value)
        {
            UpdateAction(keys, value);
        }
    }

    public class TestDataSource : DataSource<TestSetting>
    {
        public Func<IEnumerable<SettingKey>, string> SelectFunc;

        public Action<IEnumerable<SettingKey>, string> UpdateAction;

        public override string Select(IEnumerable<SettingKey> keys)
        {
            return SelectFunc(keys);
        }

        public override void Update(IEnumerable<SettingKey> keys, string value)
        {
            UpdateAction(keys, value);
        }
    }
}
