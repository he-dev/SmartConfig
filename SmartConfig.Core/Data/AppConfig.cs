using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements the app.config as data source.
    /// </summary>
    public class AppConfig : DataSource<Setting>
    {
        private IDictionary<Type, AppConfigSectionHandler> _sectionHandlers;

        public AppConfig()
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
                if (value == null) throw new ArgumentNullException("SectionHandlers");
                _sectionHandlers = value.ToDictionary(x => x.SectionType);
            }
        }

        public override string Select(string defaultKey)
        {
            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var compositeKey = new CompositeKey(defaultKey, KeyNames, KeyProperties);
            var configurationSection = GetConfigurationSection(exeConfig, compositeKey);
            var sectionHandler = _sectionHandlers[configurationSection.GetType()];
            var value = sectionHandler.Select(configurationSection, GetNameWithoutSectionName(compositeKey));
            return value;
        }

        public override void Update(string defaultKey, string value)
        {
            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var compositeKey = new CompositeKey(defaultKey, KeyNames, KeyProperties);
            var configurationSection = GetConfigurationSection(exeConfig, compositeKey);
            var sectionHandler = _sectionHandlers[configurationSection.GetType()];
            sectionHandler.Update(configurationSection, GetNameWithoutSectionName(compositeKey), value);
            exeConfig.Save(ConfigurationSaveMode.Minimal);
        }

        private static string GetSectionName(CompositeKey key)
        {
            var name = key[KeyNames.DefaultKeyName];
            var sectionName = name.Split('.').First();
            return sectionName;
        }

        private static string GetNameWithoutSectionName(IDictionary<string, string> keys)
        {
            var name = keys[SmartConfig.KeyNames.DefaultKeyName];
            name = name.Substring(name.IndexOf('.') + 1);
            return name;
        }

        private static ConfigurationSection GetConfigurationSection(Configuration configuration, CompositeKey key)
        {
            var sectionName = GetSectionName(key);
            var actualSectionName = configuration.Sections.Keys.Cast<string>().Single(x => x.Equals(sectionName, StringComparison.OrdinalIgnoreCase));
            var section = configuration.Sections[actualSectionName];
            return section;
        }
    }
}
