using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartConfig
{
    /// <summary>
    /// Provides utility methods for creating configuration element names.
    /// </summary>
    public class RegistryPath : SettingPath
    {
        private readonly SettingPath _settingPath;

        public RegistryPath(SettingPath settingPath)
        {
            Delimiter = @"\";
            _settingPath = settingPath;
        }

        public string SubKeyName => string.Join(Delimiter, _settingPath.Take(_settingPath.Length - 1));

        public string ValueName => _settingPath.Last();

        public override string ToString()
        {
            return string.Join(Delimiter, (IEnumerable<string>)_settingPath);
        }
    }
}
