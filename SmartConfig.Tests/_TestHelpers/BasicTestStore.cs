using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.Tests
{
    public class BasicTestStore : DataStore<BasicSetting>
    {
        public Func<IEnumerable<SettingKey>, string> SelectFunc;

        public Action<IEnumerable<SettingKey>, object> UpdateAction;

        public override IReadOnlyCollection<Type> SupportedSettingValueTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKeyCollection keys)
        {
            return SelectFunc(keys);
        }

        public override void Update(SettingKeyCollection keys, object value)
        {
            UpdateAction(keys, value);
        }
    }

    
}
