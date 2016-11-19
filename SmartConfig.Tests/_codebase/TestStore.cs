using System;
using System.Collections.Generic;
using System.Linq;
using SmartConfig.Data;

namespace SmartConfig.Core.Tests
{
    public class TestStore : DataStore
    {
        private readonly List<Setting> _getSettingsArguments = new List<Setting>();

        private readonly List<Setting> _saveSettingsArguments = new List<Setting>();

        private readonly DataStore _store;

        public TestStore() : base(new[] { typeof(string) }) { }

        public TestStore(DataStore store) : this()
        {
            _store = store;
        }

        public Type MapDataType(Type settingType) => typeof(string);

        public override IEnumerable<Setting> GetSettings(Setting setting)
        {
            _getSettingsArguments.Add(setting);
            return _store?.GetSettings(setting) ?? new List<Setting>();
        }       

        public override int SaveSettings(IEnumerable<Setting> settings)
        {
            _saveSettingsArguments.AddRange(settings);
            return _store?.SaveSettings(settings) ?? settings.Count();
        }

        // ---

        public IReadOnlyList<Setting> GetSettingsParameters => _getSettingsArguments;

        public IReadOnlyList<Setting> SaveSettingsParameters => _saveSettingsArguments;

    }
}