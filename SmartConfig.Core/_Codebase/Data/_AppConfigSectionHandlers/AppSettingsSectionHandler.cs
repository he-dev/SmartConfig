using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig.Data
{
    public class AppSettingsSectionHandler : AppConfigSectionHandler
    {
        public AppSettingsSectionHandler() : base(typeof(AppSettingsSection)) { }

        public override string Select(ConfigurationSection section, string key)
        {
            Debug.Assert(section is AppSettingsSection);

            var appSettings = (AppSettingsSection)section;
            var keyValueConfigurationElement = appSettings.Settings[key];
            return
                keyValueConfigurationElement == null
                ? null
                : keyValueConfigurationElement.Value;
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
