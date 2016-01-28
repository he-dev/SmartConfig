using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartConfig
{
    /// <summary>
    /// Provides utility methods for creating configuration element names.
    /// </summary>
    public class AppConfigPath : SettingPath
    {
        private readonly SettingPath _settingPath;

        public AppConfigPath(SettingPath settingPath)
        {
            _settingPath = settingPath;
        }

        public string SectionName =>
            _settingPath.ContainsConfigName
                ? _settingPath.Skip(1).First()
                : _settingPath.First();

        public override string ToString()
        {
            var path =
                _settingPath.ContainsConfigName
                    // skip section name at 1
                    ? _settingPath.Where((n, i) => i != 1)
                    // skipt section name at 0
                    : _settingPath.Skip(1);

            return string.Join(Delimiter, path);
        }
    }
}
