using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.DataStores
{
    public class MemoryStore : IDataStore, IEnumerable<Setting>
    {
        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(SettingPath path, IReadOnlyDictionary<string, object> namespaces)
        {
            var settings =
                from x in Data
                where x.Name.IsMatch(path) && (namespaces == null || namespaces.All(n => x[n.Key] == n.Value))
                select x;

            return settings.ToList();
        }

        public int SaveSetting(SettingPath path, IReadOnlyDictionary<string, object> namespaces, object value)
        {
            return SaveSettings(new Dictionary<SettingPath, object> { { path, value } }, namespaces);
        }

        public int SaveSettings(IReadOnlyDictionary<SettingPath, object> settings, IReadOnlyDictionary<string, object> namespaces)
        {
            foreach (var setting in settings)
            {
                var removeSettings = GetSettings(setting.Key, namespaces);
                foreach (var removeSetting in removeSettings)
                {
                    Data.Remove(removeSetting);
                }
            }

            foreach (var setting in settings)
            {
                Add(setting.Key, setting.Value);
            }

            return settings.Count;
        }

        public List<Setting> Data { get; set; } = new List<Setting>();

        public void Add(Setting setting) => Data.Add(setting);

        public void Add(string name, object value) => Data.Add(new Setting { Name = name, Value = value });

        public IEnumerator<Setting> GetEnumerator() => Data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
