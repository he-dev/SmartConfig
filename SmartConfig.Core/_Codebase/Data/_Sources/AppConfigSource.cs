using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using SmartConfig.Collections;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements the app.configuration as data source.
    /// </summary>
    public class AppConfigSource : DataSource<Setting>
    {
        private IDictionary<Type, IAppConfigSectionHandler> _sectionHandlers;

        public AppConfigSource()
        {
            _sectionHandlers = new object[]
            {
                new AppSettingsSectionHandler(),
                new ConnectionStringsSectionHandler(),
            }
            .Cast<IAppConfigSectionHandler>()
            .ToDictionary(x => x.SectionType, x => x);
        }

        /// <summary>
        /// Gets or sets section handlers.
        /// </summary>
        public IEnumerable<IAppConfigSectionHandler> SectionHandlers
        {
            get { return _sectionHandlers.Values.ToList(); }
            set
            {
                if (value == null || !value.Any())
                {
                    throw new ArgumentNullException(nameof(SectionHandlers), "There must be at least one section handler.");
                }
                _sectionHandlers = value.ToDictionary(x => x.SectionType);
            }
        }

        public IEnumerable<string> SectionNames => _sectionHandlers.Values.Select(sh => sh.SectionName);

        private ConfigurationSection GetConfigurationSection(System.Configuration.Configuration configuration, SettingKey defaultKey)
        {
            Debug.Assert(configuration != null);
            Debug.Assert(defaultKey != null);

            var sectionName = new AppConfigPath((SettingPath)defaultKey.Value, SectionNames).SectionName;
            var actualSectionName = configuration.Sections.Keys.Cast<string>().Single(x => x.Equals(sectionName, StringComparison.OrdinalIgnoreCase));
            var section = configuration.Sections[actualSectionName];
            return section;
        }

        public override IReadOnlyCollection<Type> SupportedTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKeyReadOnlyCollection keys)
        {
            Debug.Assert(keys != null && keys.Any());

            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = GetConfigurationSection(exeConfig, keys.DefaultKey);
            var sectionHandler = _sectionHandlers[configurationSection.GetType()];
            var settingName = new AppConfigPath((SettingPath)keys.DefaultKey.Value, SectionNames).ToString();
            var value = sectionHandler.Select(configurationSection, settingName);
            return value;
        }

        public override void Update(SettingKeyReadOnlyCollection keys, object value)
        {
            Debug.Assert(keys != null && keys.Any());

            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = GetConfigurationSection(exeConfig, keys.DefaultKey);
            var sectionHandler = _sectionHandlers[configurationSection.GetType()];
            var settingName = new AppConfigPath((SettingPath)keys.DefaultKey.Value, SectionNames).ToString();
            sectionHandler.Update(configurationSection, settingName, value?.ToString());
            exeConfig.Save(ConfigurationSaveMode.Minimal);
        }
    }
}
