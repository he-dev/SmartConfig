using System;
using System.Collections.Generic;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests.DataStores
{
    public class TestStore : IDataStore
    {
        private readonly List<Setting> _getSettingsArguments = new List<Setting>();

        private readonly List<Setting> _saveSettingsArguments = new List<Setting>();

        private readonly IDataStore _store;

        public TestStore() { }

        public TestStore(IDataStore store)
        {
            _store = store;
        }

        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(Setting setting)
        {
            _getSettingsArguments.Add(setting);
            return _store?.GetSettings(setting) ?? new List<Setting>();
        }

        public int SaveSetting(Setting setting)
        {
            return SaveSettings(new[] { setting });
        }

        public int SaveSettings(IReadOnlyCollection<Setting> settings)
        {
            _saveSettingsArguments.AddRange(settings);
            return _store?.SaveSettings(settings) ?? settings.Count;
        }

        // ---

        public IReadOnlyList<Setting> GetSettingsParameters => _getSettingsArguments;

        public IReadOnlyList<Setting> SaveSettingsParameters => _saveSettingsArguments;

    }
}