﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests
{
    public class BasicTestStore : DataStore<BasicSetting>
    {
        public Func<CompoundSettingKey, string> SelectFunc;

        public Action<CompoundSettingKey, object> UpdateAction;

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
