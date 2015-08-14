using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public class AppSettingsSectionHandler : AppConfigSectionHandler
    {
        public AppSettingsSectionHandler() : base(typeof(AppSettingsSection)) { }

        public override string Select(ConfigurationSection section, string key)
        {
            var appSettings = (section as AppSettingsSection);
            var actualKey = GetActualKey(appSettings, key);
            return string.IsNullOrEmpty(actualKey) ? null : appSettings.Settings[actualKey].Value;
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
            // https://regex101.com/r/gP8mK8/1
            //var keyPattern = @"(?<Key>[a-z][a-z0-9_]+)(@(?<OtherKey>[a-z][a-z0-9_]+):(?<Value>[a-z0-9$._-]+))?";
            var keyPattern = @"(?<Key>" + key + ")(@(?<OtherKey>[a-z][a-z0-9_]+):(?<Value>[a-z0-9$._-]+))?";

            // select all keys that match the specified key
            var matches = section.Settings.AllKeys
                .Select(k => Regex.Match(k, keyPattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture))
                .Where(m => m.Success);

            // select all matches with 'OtherKey'
            var otherKeyMatches = matches.Where(m => m.Groups["OtherKey"].Success);

            var otherKeyGroup = otherKeyMatches
                // group by 'OtherKey' and get their values
                .GroupBy(m => m.Groups["OtherKey"].Value, m => m.Groups["Value"].Value)
                // at most o only one such group can exist
                .SingleOrDefault();

            if (otherKeyGroup != null)
            {
                // get the actual key for the other key
                var actualOtherKey = section.Settings.AllKeys.SingleOrDefault(k => k.Equals(otherKeyGroup.Key, StringComparison.InvariantCulture));

                // get the other key's value
                var otherKeyValue = section.Settings[actualOtherKey].Value;

                // build the actual conditional key
                var actualConditionalKey = string.Format("{0}@{1}:{2}", key, otherKeyGroup.Key, otherKeyValue);
                return actualConditionalKey;
            }

            var actualKey = section.Settings.AllKeys.SingleOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
            return actualKey;
        }
    }
}
