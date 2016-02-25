using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.DataStores.AppConfig
{
    /// <summary>
    /// Implements the app.configuration as data source.
    /// </summary>
    public class AppConfigStore : DataStore<BasicSetting>
    {
        private readonly System.Configuration.Configuration _exeConfiguration;

        private readonly IDictionary<string, IAppConfigSectionStore> _appConfigSectionSources;

        public AppConfigStore()
        {
            _exeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            _appConfigSectionSources = new object[]
            {
                new AppSettingsSectionStore(_exeConfiguration.AppSettings),
                new ConnectionStringsSectionStore(_exeConfiguration.ConnectionStrings),
            }
            .Cast<IAppConfigSectionStore>()
            .ToDictionary(x => x.SectionName, x => x, StringComparer.OrdinalIgnoreCase);
        }

        private IAppConfigSectionStore GetAppConfigSectionStore(NameKey nameKey)
        {
            Debug.Assert(nameKey != null);

            var sectionName = new AppConfigPath(nameKey.Value).SectionName;

            IAppConfigSectionStore appConfigSectionStore;
            if (!_appConfigSectionSources.TryGetValue(sectionName, out appConfigSectionStore))
            {
                // todo: throw section not found exception
            }

            return appConfigSectionStore;
        }

        public override IReadOnlyCollection<Type> SupportedSettingDataTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(CompoundSettingKey keys)
        {
            if (keys == null) { throw new ArgumentNullException(nameof(keys)); }
            if (!keys.Any()) { throw new InvalidOperationException("There must be at least one key defined."); }

            var appConfigSectionSource = GetAppConfigSectionStore(keys.NameKey);

            var settingName = new AppConfigPath(keys.NameKey.Value).ToString();
            var value = appConfigSectionSource.Select(settingName);

            return value;
        }

        public override void Update(CompoundSettingKey keys, object value)
        {
            Debug.Assert(keys != null && keys.Any());

            var appConfigSectionSource = GetAppConfigSectionStore(keys.NameKey);
            var settingName = new AppConfigPath(keys.NameKey.Value).ToString();

            appConfigSectionSource.Update(settingName, value?.ToString());
            _exeConfiguration.Save(ConfigurationSaveMode.Minimal);
        }
    }
}
