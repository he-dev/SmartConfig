using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public class AppSettingsSectionHanlder : AppConfigSectionHanlder
    {
        public override Type SectionType { get { return typeof(AppSettingsSection); } }

        public override string Select(ConfigurationSection section, string key)
        {
            var appSettings = (section as AppSettingsSection);
            var actualKey = GetActualKey(appSettings, key);
            return string.IsNullOrEmpty(actualKey) ? null : appSettings.Settings[key].Value;
        }

        public override void Update(ConfigurationSection section, string key, string value)
        {
            var appSettings = (section as AppSettingsSection);
            var actualKey = GetActualKey(appSettings, key);
            if (!string.IsNullOrEmpty(actualKey))
            {
                appSettings.Settings.Remove(actualKey);
            }
            appSettings.Settings.Add(key, value);
        }

        private static string GetActualKey(AppSettingsSection section, string key)
        {
            var actualKey = section.Settings.AllKeys.SingleOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
            return actualKey;
        }
    }
}
