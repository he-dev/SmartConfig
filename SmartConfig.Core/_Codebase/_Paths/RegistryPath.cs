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

        public override IReadOnlyCollection<string> Names => _settingPath.Names;

        public string SubKeyName => string.Join(Delimiter, Names.Take(Names.Count - 1));

        public string ValueName => Names.Last();

        public override string ToString()
        {
            return string.Join(Delimiter, _settingPath.Names);
        }
    }
}
