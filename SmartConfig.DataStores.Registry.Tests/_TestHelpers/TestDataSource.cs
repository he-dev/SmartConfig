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
        public Func<SettingKey, string> SelectFunc;

        public Action<SettingKey, object> UpdateAction;

        public override IReadOnlyCollection<Type> SerializationDataTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKey key)
        {
            return SelectFunc(key);
        }

        public override void Update(SettingKey key, object value)
        {
            UpdateAction(key, value);
        }
    }

    public class TestDataSource : DataStore<TestSetting>
    {
        public Func<SettingKey, string> SelectFunc;

        public Action<SettingKey, object> UpdateAction;

        public override IReadOnlyCollection<Type> SerializationDataTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKey key)
        {
            return SelectFunc(key);
        }

        public override void Update(SettingKey key, object value)
        {
            UpdateAction(key, value);
        }
    }
}
