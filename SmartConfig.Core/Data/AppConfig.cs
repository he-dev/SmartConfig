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
    public class AppConfig : DataSource<ConfigElement>
    {
        // https://regex101.com/r/vA9kR5/3
        //private static readonly string SectionNamePattern = @"^(?:[A-Z0-9_]+\.)?(?:(?<SectionName>ConnectionStrings|AppSettings)\.)";

        public IDictionary<Type, AppConfigSectionHandler> SectionHandlers { get; private set; }

        public AppConfig()
        {
            SectionHandlers = new AppConfigSectionHandler[]
            {
                new AppSettingsSectionHandler(),
                new ConnectionStringsSectionHandler(),
            }
            .ToDictionary(x => x.SectionType, x => x);
        }

        public AppConfig(IEnumerable<AppConfigSectionHandler> sectionHandlers) : base()
        {
            foreach (var sectionHandler in sectionHandlers)
            {
                SectionHandlers.Add(sectionHandler.SectionType, sectionHandler);
            }
        }

        public override string Select(IDictionary<string, string> keys)
        {
            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = GetConfigurationSection(exeConfig, keys);
            var sectionHandler = SectionHandlers[configurationSection.GetType()];
            var value = sectionHandler.Select(configurationSection, GetNameWithoutSectionName(keys));
            return value;
        }

        public override void Update(IDictionary<string, string> keys, string value)
        {
            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = GetConfigurationSection(exeConfig, keys);
            var sectionHandler = SectionHandlers[configurationSection.GetType()];
            sectionHandler.Update(configurationSection, GetNameWithoutSectionName(keys), value);
            exeConfig.Save(ConfigurationSaveMode.Minimal);
        }

        private static string GetSectionName(IDictionary<string, string> keys)
        {
            var name = keys[KeyNames.DefaultKeyName];
            var sectionName = name.Split('.').First();
            return sectionName;
        }

        private static string GetNameWithoutSectionName(IDictionary<string, string> keys)
        {
            var name = keys[KeyNames.DefaultKeyName];
            name = name.Substring(name.IndexOf('.') + 1);
            return name;
        }

        private static ConfigurationSection GetConfigurationSection(Configuration configuration, IDictionary<string, string> keys)
        {
            var sectionName = GetSectionName(keys);
            var actualSectionName = configuration.Sections.Keys.Cast<string>().Single(x => x.Equals(sectionName, StringComparison.OrdinalIgnoreCase));
            var section = configuration.Sections[actualSectionName];
            return section;
        }
    }
}
