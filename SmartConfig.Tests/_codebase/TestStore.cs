using System;
using System.Collections.Generic;
using System.Linq;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests
{
    public class TestStore : DataStore
    {
        //private readonly DataStore _store;

        public TestStore() : base(new[] { typeof(string) }) { }

        //public TestStore(DataStore store) : this()
        //{
        //    _store = store;
        //}

        public override IEnumerable<Setting> GetSettings(Setting setting)
        {
            return GetSettingsCallback(setting);
        }       

        public override int SaveSettings(IEnumerable<Setting> settings)
        {
            return SaveSettingsCallback(settings);
        }

        // ---

        public Func<Setting, IEnumerable<Setting>> GetSettingsCallback { get; set; }

        public Func<IEnumerable<Setting>, int> SaveSettingsCallback { get; set; }
    }
}