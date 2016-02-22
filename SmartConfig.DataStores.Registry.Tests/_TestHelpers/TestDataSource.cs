using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Tests;

namespace SmartConfig.DataStores.Registry.Tests
{
    public class SimpleTestDataSource : DataStore<BasicSetting>
    {
        public Func<IEnumerable<SimpleSettingKey>, string> SelectFunc;

        public Action<IEnumerable<SimpleSettingKey>, object> UpdateAction;

        public override IReadOnlyCollection<Type> SupportedSettingDataTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(CompoundSettingKey keys)
        {
            return SelectFunc(keys);
        }

        public override void Update(CompoundSettingKey keys, object value)
        {
            UpdateAction(keys, value);
        }
    }

    public class TestDataSource : DataStore<TestSetting>
    {
        public Func<IEnumerable<SimpleSettingKey>, string> SelectFunc;

        public Action<IEnumerable<SimpleSettingKey>, object> UpdateAction;

        public override IReadOnlyCollection<Type> SupportedSettingDataTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(CompoundSettingKey keys)
        {
            return SelectFunc(keys);
        }

        public override void Update(CompoundSettingKey keys, object value)
        {
            UpdateAction(keys, value);
        }
    }
}
