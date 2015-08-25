using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
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
                if (value == null || !value.Any()) throw new ArgumentNullException("SectionHandlers", "There must be at least one section handler.");
                _sectionHandlers = value.ToDictionary(x => x.SectionType);
            }
        }

        public override string Select(string defaultKeyValue)
        {
            if (string.IsNullOrEmpty(defaultKeyValue)) throw new ArgumentNullException("defaultKeyValue");

            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var compositeKey = CreateCompositeKey(defaultKeyValue);
            var configurationSection = GetConfigurationSection(exeConfig, compositeKey);
            var sectionHandler = _sectionHandlers[configurationSection.GetType()];
            var value = sectionHandler.Select(configurationSection, GetNameWithoutSectionName(compositeKey));
            return value;
        }

        public override void Update(string defaultKeyValue, string value)
        {
            if (string.IsNullOrEmpty(defaultKeyValue)) throw new ArgumentNullException("defaultKeyValue");

            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var compositeKey = CreateCompositeKey(defaultKeyValue);
            var configurationSection = GetConfigurationSection(exeConfig, compositeKey);
            var sectionHandler = _sectionHandlers[configurationSection.GetType()];
            sectionHandler.Update(configurationSection, GetNameWithoutSectionName(compositeKey), value);
            exeConfig.Save(ConfigurationSaveMode.Minimal);
        }

        private static string GetSectionName(CompositeKey compositeKey)
        {
            Debug.Assert(compositeKey != null);

            var name = compositeKey[KeyNames.DefaultKeyName];
            var sectionName = name.Split('.').First();
            return sectionName;
        }

        private static string GetNameWithoutSectionName(IDictionary<string, string> keys)
        {
            Debug.Assert(keys != null);

            var name = keys[KeyNames.DefaultKeyName];
            name = name.Substring(name.IndexOf('.') + 1);
            return name;
        }

        private static ConfigurationSection GetConfigurationSection(Configuration configuration, CompositeKey compositeKey)
        {
            Debug.Assert(configuration != null);
            Debug.Assert(compositeKey != null);

            var sectionName = GetSectionName(compositeKey);
            var actualSectionName = configuration.Sections.Keys.Cast<string>().Single(x => x.Equals(sectionName, StringComparison.OrdinalIgnoreCase));
            var section = configuration.Sections[actualSectionName];
            return section;
        }
    }
}
