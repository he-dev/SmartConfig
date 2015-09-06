using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements the app.config as data source.
    /// </summary>
    public class AppConfigSource : DataSource<Setting>
    {
        private IDictionary<Type, AppConfigSectionHandler> _sectionHandlers;

        public AppConfigSource()
        {
            _sectionHandlers = new AppConfigSectionHandler[]
            {
                new AppSettingsSectionHandler(),
                new ConnectionStringsSectionHandler(),
            }
            .ToDictionary(x => x.SectionType, x => x);
        }      

        /// <summary>
        /// Gets or sets section handlers.
        /// </summary>
        public IEnumerable<AppConfigSectionHandler> SectionHandlers
        {
            get { return _sectionHandlers.Values.ToList(); }
            set
            {
                if (value == null || !value.Any())
                {
                    throw new ArgumentNullException("SectionHandlers", "There must be at least one section handler.");
                }
                _sectionHandlers = value.ToDictionary(x => x.SectionType);
            }
        }

        public override string Select(string defaultKeyValue)
        {
            if (string.IsNullOrEmpty(defaultKeyValue)) throw new ArgumentNullException(nameof(defaultKeyValue));

            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var compositeKey = CreateCompositeKey(defaultKeyValue);
            var configurationSection = GetConfigurationSection(exeConfig, compositeKey);
            var sectionHandler = _sectionHandlers[configurationSection.GetType()];
            var value = sectionHandler.Select(configurationSection, GetSettingName(compositeKey));
            return value;
        }

        public override void Update(string defaultKeyValue, string value)
        {
            if (string.IsNullOrEmpty(defaultKeyValue)) throw new ArgumentNullException(nameof(defaultKeyValue));

            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var compositeKey = CreateCompositeKey(defaultKeyValue);
            var configurationSection = GetConfigurationSection(exeConfig, compositeKey);
            var sectionHandler = _sectionHandlers[configurationSection.GetType()];
            sectionHandler.Update(configurationSection, GetSettingName(compositeKey), value);
            exeConfig.Save(ConfigurationSaveMode.Minimal);
        }

        private static ConfigurationSection GetConfigurationSection(System.Configuration.Configuration configuration, CompositeKey compositeKey)
        {
            Debug.Assert(configuration != null);
            Debug.Assert(compositeKey != null);

            var sectionName = GetSectionName(compositeKey);
            var actualSectionName = configuration.Sections.Keys.Cast<string>().Single(x => x.Equals(sectionName, StringComparison.OrdinalIgnoreCase));
            var section = configuration.Sections[actualSectionName];
            return section;
        }

        private static string GetSectionName(CompositeKey compositeKey)
        {
            Debug.Assert(compositeKey != null);

            var sectionName = compositeKey.DefaultKey.Substring(0, compositeKey.DefaultKey.IndexOf('.'));
            return sectionName;
        }

        private static string GetSettingName(CompositeKey compositeKey)
        {
            Debug.Assert(compositeKey != null);

            var settingName = compositeKey.DefaultKey.Substring(compositeKey.DefaultKey.IndexOf('.') + 1);
            return settingName;
        }
    }
}
