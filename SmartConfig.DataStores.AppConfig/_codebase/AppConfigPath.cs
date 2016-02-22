using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartConfig.DataStores.AppConfig
{
    /// <summary>
    /// Provides utility methods for creating configuration element names.
    /// </summary>
    public class AppConfigPath
    {
        private readonly SettingPath _settingPath;

        public AppConfigPath(SettingPath settingPath)
        {
            if (settingPath == null) { throw new ArgumentNullException(nameof(settingPath)); }
            _settingPath = settingPath;
        }

        public AppConfigPath(SimpleSettingKey simpleKey) : this(simpleKey.Value as SettingPath) { }

        public string SectionName => _settingPath.WithoutConfigurationName.FirstOrDefault();

        public IEnumerable<string> AfterSectionName => _settingPath.WithoutConfigurationName.Skip(1);

        public override string ToString()
        {
            var path = new List<string>();
            if (_settingPath.ContainsConfigurationName)
            {
                path.Add(_settingPath.ConfigurationName);
            }
            path.AddRange(AfterSectionName);

            return string.Join(_settingPath.Delimiter, path);
        }

        public static implicit operator string(AppConfigPath appConfigPath)
        {
            return appConfigPath.ToString();
        }
    }
}
