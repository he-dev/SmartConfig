using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests
{
    public class TestStore : IDataStore
    {
        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(SettingPath path, IReadOnlyDictionary<string, object> namespaces)
            => GetSettingsFunc(path, namespaces);

        public int SaveSetting(SettingPath path, IReadOnlyDictionary<string, object> namespaces, object value)
            => UpdateSettingFunc(path, namespaces, value);

        public int SaveSettings(IReadOnlyDictionary<SettingPath, object> settings, IReadOnlyDictionary<string, object> namespaces)
        {
            throw new NotImplementedException();
        }

        // --- test helpers

        public Func<SettingPath, IReadOnlyDictionary<string, object>, List<Setting>> GetSettingsFunc;

        public Func<SettingPath, IReadOnlyDictionary<string, object>, object, int> UpdateSettingFunc;
    }

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
