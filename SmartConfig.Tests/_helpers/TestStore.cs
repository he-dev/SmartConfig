using System;
using System.Collections.Generic;
using System.Linq;
using SmartConfig.Data;
using SmartConfig.DataStores;

namespace SmartConfig.Core.Tests
{
    public class TestStore : MemoryStore
    {
        public TestStore() : base(new[] { typeof(string) }) { }

        public override IEnumerable<Setting> ReadSettings(Setting setting)
        {
            return ReadSettingsCallback(setting);
        }

        protected override void WriteSettings(ICollection<IGrouping<Setting, Setting>> settings) => WriteSettingsCallback(settings);

        // ---

        public Func<Setting, IEnumerable<Setting>> ReadSettingsCallback { get; set; }

        public Action<ICollection<IGrouping<Setting, Setting>>> WriteSettingsCallback { get; set; }
    }
}