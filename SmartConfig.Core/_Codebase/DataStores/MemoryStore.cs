using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.DataStores
{
    // Provides a memory-store.
    public class MemoryStore : DataStore, IEnumerable<Setting>
    {
        public MemoryStore()
            : base(new[] { typeof(string) })
        { }

        protected MemoryStore(IEnumerable<Type> supportedTypes)
            : base(supportedTypes)
        { }

        public override IEnumerable<Setting> ReadSettings(Setting setting)
        {
            return Data.Like(setting);
        }

        protected override void WriteSettings(ICollection<IGrouping<Setting, Setting>> settings)
        {
            foreach (var grouping in settings)
            {
                var obsoleteSettings = Data.Like(grouping.Key).ToList();
                obsoleteSettings.ForEach(x => Data.Remove(x));

                foreach (var setting in grouping)
                {
                    Add(setting.Name.StrongFullName, setting.Value);
                }
            }
        }

        public List<Setting> Data { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = new List<Setting>();

        #region IEnumerable

        public void Add(Setting setting) => Data.Add(setting);

        public void Add(string name, object value) => Data.Add(new Setting
        {
            Name = SettingPath.Parse(name),
            Value = value
        });

        public IEnumerator<Setting> GetEnumerator() => Data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
