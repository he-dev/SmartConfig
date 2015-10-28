using System.Configuration;
using System.Diagnostics;

namespace SmartConfig.Data
{
    public class AppSettingsSectionHandler : AppConfigSectionHandler<AppSettingsSection>, IAppConfigSectionHandler
    {
        public override string Select(ConfigurationSection section, string key)
        {
            Debug.Assert(section is AppSettingsSection);

            var appSettings = (AppSettingsSection)section;
            var keyValueConfigurationElement = appSettings.Settings[key];
            return keyValueConfigurationElement?.Value;
        }

        public override void Update(ConfigurationSection section, string key, string value)
        {
            Debug.Assert(section is AppSettingsSection);

            var appSettings = (AppSettingsSection)section;
            var keyValueConfigurationElement = appSettings.Settings[key];
            if (keyValueConfigurationElement == null)
            {
                appSettings.Settings.Add(key, value);
            }
            else
            {
                keyValueConfigurationElement.Value = value;
            }
        }
    }
}
