using System;
using System.Linq;

namespace SmartConfig.Paths
{
    /// <summary>
    /// Provides utility methods for creating configuration element names.
    /// </summary>
    public class AppConfigPath
    {
        private const int SectionNameIndex = 1;

        private readonly SettingPath _settingPath;

        public AppConfigPath(SettingPath settingPath)
        {
            if (settingPath == null) { throw new ArgumentNullException(nameof(settingPath)); }
            _settingPath = settingPath;
        }

        public AppConfigPath(SettingKey settingKey) : this(settingKey.Value as SettingPath) { }

        public string SectionName => _settingPath.SkipWhile((n, i) => i == SettingPath.ConfigurationNameIndex).First();

        public override string ToString()
        {
            var path =
                string.IsNullOrEmpty(_settingPath.ConfigurationName)
                    ? _settingPath.Where(((n, i) => i != SettingPath.ConfigurationNameIndex && i != SectionNameIndex))
                    : _settingPath.Where((n, i) => i != SectionNameIndex);

            return string.Join(_settingPath.Delimiter, path);
        }

        public static implicit operator string(AppConfigPath appConfigPath)
        {
            return appConfigPath.ToString();
        }
    }
}
