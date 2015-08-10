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
    public class AppConfig : IDataSource
    {
        // https://regex101.com/r/vA9kR5/3
        //private static readonly string SectionNamePattern = @"^(?:[A-Z0-9_]+\.)?(?:(?<SectionName>ConnectionStrings|AppSettings)\.)";

        private static class SectionNames
        {
            public const string ConnectionStrings = "ConnectionStrings";
            public const string AppSettings = "AppSettings";
        }

        public IDictionary<Type, AppConfigSectionHanlder> SectionHanlders { get; private set; }

        public AppConfig()
        {
            SectionHanlders = new AppConfigSectionHanlder[]
            {
                new AppSettingsSectionHanlder(),
                new ConnectionStringsSectionHanlder(),
            }
            .ToDictionary(x => x.SectionType, x => x);
        }

        public string Select(IDictionary<string, string> keys)
        {
            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = GetConfigurationSection(exeConfig, keys);
            var sectionHandler = SectionHanlders[configurationSection.GetType()];
            var value = sectionHandler.Select(configurationSection, GetNameWithoutSectionName(keys));
            return value;
        }

        public void Update(IDictionary<string, string> keys, string value)
        {
            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = GetConfigurationSection(exeConfig, keys);
            var sectionHandler = SectionHanlders[configurationSection.GetType()];
            sectionHandler.Update(configurationSection, GetNameWithoutSectionName(keys), value);
            exeConfig.Save(ConfigurationSaveMode.Minimal);
        }

        private static string GetSectionName(IDictionary<string, string> keys)
        {
            var name = keys[CommonFieldKeys.Name];
            var sectionName = name.Split('.').First();
            return sectionName;
        }

        private static string GetNameWithoutSectionName(IDictionary<string, string> keys)
        {
            var name = keys[CommonFieldKeys.Name];
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
