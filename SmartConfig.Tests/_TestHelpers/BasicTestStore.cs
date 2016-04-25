using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests
{
    public class BasicTestStore : DataStore<BasicSetting>
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
