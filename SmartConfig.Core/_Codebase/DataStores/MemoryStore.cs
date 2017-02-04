using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SmartConfig.Data;

namespace SmartConfig.DataStores
{
    // Provides a memory-store.
    public class MemoryStore : DataStore, IEnumerable<Setting>
    {
        public MemoryStore() : base(new[] { typeof(string) }) { }

        public override IEnumerable<Setting> GetSettings(Setting setting)
        {
            var settings =
                (from x in Data
                     //where x.Name.IsMatch(path) && (namespaces == null || namespaces.All(n => x.NamespaceEquals(n.Key, n.Value)))
                 where x.Name.IsLike(setting.Name)
                 select x).ToList();

            return settings;
        }

        public override int SaveSettings(IEnumerable<Setting> settings)
        {
            foreach (var setting in settings)
            {
                var removeSettings = GetSettings(setting);
                foreach (var removeSetting in removeSettings)
                {
                    Data.Remove(removeSetting);
                }
            }

            foreach (var setting in settings)
            {
                Add(setting.Name.StrongFullName, setting.Value);
            }

            return settings.Count();
        }

        public List<Setting> Data { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = new List<Setting>();

        public void Add(Setting setting) => Data.Add(setting);

        public void Add(string name, object value) => Data.Add(new Setting
        {
            Name = SettingPath.Parse(name),
            Value = value
        });

        public IEnumerator<Setting> GetEnumerator() => Data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
