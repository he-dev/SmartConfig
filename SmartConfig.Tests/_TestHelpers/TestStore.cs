using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests
{
    public class TestStore<TSetting> : DataStore<TSetting> where TSetting : BasicSetting, new()
    {
        public Func<SettingKey, string> SelectFunc;

        public Action<SettingKey, object> UpdateAction;

        public override IReadOnlyCollection<Type> SerializationTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKey key)
        {
            return SelectFunc?.Invoke(key);
        }

        public override void Update(SettingKey key, object value)
        {
            UpdateAction?.Invoke(key, value);
        }
    }


}
