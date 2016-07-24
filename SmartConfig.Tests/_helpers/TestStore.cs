using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SmartConfig;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests
{
    //public class TestStore : IDataStore, IEnumerable<Setting>
    //{
    //    public Type MapDataType(Type settingType) => typeof(string);

    //    public List<Setting> GetSettings(SettingPath path, IReadOnlyDictionary<string, object> namespaces)
    //        => GetSettingsFunc(path, namespaces);

    //    public int SaveSetting(SettingPath path, IReadOnlyDictionary<string, object> namespaces, object value)
    //        => UpdateSettingFunc(path, namespaces, value);

    //    public int SaveSettings(IReadOnlyDictionary<SettingPath, object> settings, IReadOnlyDictionary<string, object> namespaces)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    // --- test helpers

    //    public TestStore()
    //    {
    //        GetSettingsFunc = (path, namespaces) => Data.Where(x => x.Name.ToString().Equals(path, StringComparison.OrdinalIgnoreCase)).ToList();
    //    }

    //    public List<Setting> Data { get; set; } = new List<Setting>();

    //    public Func<SettingPath, IReadOnlyDictionary<string, object>, List<Setting>> GetSettingsFunc { get; set; }

    //    public Func<SettingPath, IReadOnlyDictionary<string, object>, object, int> UpdateSettingFunc;

    //    public void Add(Setting setting) => Data.Add(setting);

    //    public void Add(string name, object value) => Data.Add(new Setting { Name = name, Value = value });

    //    public IEnumerator<Setting> GetEnumerator() => Data.GetEnumerator();

    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    //}

    public class NullStore : IDataStore
    {
        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(SettingPath path, IReadOnlyDictionary<string, object> customProperties)
            => Enumerable.Empty<Setting>().ToList();

        public int SaveSetting(SettingPath path, IReadOnlyDictionary<string, object> customProperties, object value)
            => 0;

        public int SaveSettings(IReadOnlyDictionary<SettingPath, object> settings, IReadOnlyDictionary<string, object> namespaces)
            => 0;
    }

    public static class TestUtilities
    {
        public static IEnumerable<T> AsEnumerable<T>(this T value)
        {
            return Enumerable.Repeat(value, 1);
        }
    }

}
