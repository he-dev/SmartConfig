using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.DataStores
{
    public class MemoryStore : IDataStore, IEnumerable<Setting>
    {
        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(Setting setting)
        {
            var settings =
                (from x in Data
                 //where x.Name.IsMatch(path) && (namespaces == null || namespaces.All(n => x.NamespaceEquals(n.Key, n.Value)))
                 where x.IsLike(setting)
                 select x).ToList();

            return settings;
        }

        public int SaveSetting(Setting setting)
        {
            return SaveSettings(new [] {setting});
        }

        public int SaveSettings(IReadOnlyCollection<Setting> settings)
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
                Add(setting.Name.FullNameEx, setting.Value);
            }

            return settings.Count;
        }

        public List<Setting> Data { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = new List<Setting>();

        public void Add(Setting setting) => Data.Add(setting);

        public void Add(string name, object value) => Data.Add(new Setting { Name = name, Value = value });

        public IEnumerator<Setting> GetEnumerator() => Data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
