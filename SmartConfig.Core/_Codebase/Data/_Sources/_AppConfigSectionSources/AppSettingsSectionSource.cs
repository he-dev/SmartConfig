using System;
using System.Configuration;
using System.Diagnostics;

namespace SmartConfig.Data
{
    public class AppSettingsSectionSource : AppConfigSectionSource<AppSettingsSection>, IAppConfigSectionSource
    {
        public AppSettingsSectionSource(AppSettingsSection appSettingsSection) : base(appSettingsSection)
        {

        }

        public string SectionName => ConfigurationSection.SectionInformation.Name;

        public override string Select(string key)
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }

            var keyValueConfigurationElement = ConfigurationSection.Settings[key];
            return keyValueConfigurationElement?.Value;
        }

        public override void Update(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }

            var keyValueConfigurationElement = ConfigurationSection.Settings[key];
            if (keyValueConfigurationElement == null)
            {
                ConfigurationSection.Settings.Add(key, value);
            }
            else
            {
                keyValueConfigurationElement.Value = value;
            }
        }
    }
}
