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

        private IAppConfigSectionStore GetAppConfigSectionStore(SettingPath path)
        {
            Debug.Assert(path != null);

            var sectionName = new AppConfigPath(path).SectionName;

            IAppConfigSectionStore appConfigSectionStore;
            if (!_appConfigSectionSources.TryGetValue(sectionName, out appConfigSectionStore))
            {
                // todo: throw section not found exception
            }

            return appConfigSectionStore;
        }

        public override IReadOnlyCollection<Type> SerializationTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKey key)
        {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            //if (!key.Any()) { throw new InvalidOperationException("There must be at least one key defined."); }

            var appConfigSectionSource = GetAppConfigSectionStore(key.Main.Value);

            var settingName = new AppConfigPath(key.Main.Value).ToString();
            var value = appConfigSectionSource.Select(settingName);

            return value;
        }

        public override void Update(SettingKey key, object value)
        {
            Debug.Assert(key != null && key.Any());

            var appConfigSectionSource = GetAppConfigSectionStore(key.Main.Value);
            var settingName = new AppConfigPath(key.Main.Value).ToString();

            appConfigSectionSource.Update(settingName, value?.ToString());
            _exeConfiguration.Save(ConfigurationSaveMode.Minimal);
        }
    }
}
