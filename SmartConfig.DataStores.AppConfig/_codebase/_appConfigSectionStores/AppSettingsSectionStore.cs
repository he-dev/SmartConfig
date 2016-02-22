using System;
using System.Configuration;
using SmartConfig.Data;

namespace SmartConfig.DataStores.AppConfig
{
    public class AppSettingsSectionStore : AppConfigSectionStore<AppSettingsSection>, IAppConfigSectionStore
    {
        public AppSettingsSectionStore(AppSettingsSection appSettingsSection) : base(appSettingsSection)
        {

        }

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
