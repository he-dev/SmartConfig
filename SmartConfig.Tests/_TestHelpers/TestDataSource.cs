using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.Tests
{
    public class SimpleTestDataSource : DataSource<Setting>
    {
        public Func<IEnumerable<SettingKey>, string> SelectFunc;

        public Action<IEnumerable<SettingKey>, object> UpdateAction;

        public override IReadOnlyCollection<Type> SupportedTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKeyReadOnlyCollection keys)
        {
            return SelectFunc(keys);
        }

        public override void Update(SettingKeyReadOnlyCollection keys, object value)
        {
            UpdateAction(keys, value);
        }
    }

    public class TestDataSource : DataSource<TestSetting>
    {
        public Func<IEnumerable<SettingKey>, string> SelectFunc;

        public Action<IEnumerable<SettingKey>, object> UpdateAction;

        public override IReadOnlyCollection<Type> SupportedTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKeyReadOnlyCollection keys)
        {
            return SelectFunc(keys);
        }

        public override void Update(SettingKeyReadOnlyCollection keys, object value)
        {
            UpdateAction(keys, value);
        }
    }
}
