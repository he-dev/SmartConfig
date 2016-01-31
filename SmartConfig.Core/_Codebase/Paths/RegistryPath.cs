using System.Collections.Generic;
using System.Linq;

namespace SmartConfig.Paths
{
    public class RegistryPath
    {
        private readonly SettingPath _settingPath;

        public RegistryPath(SettingPath settingPath)
        {
            _settingPath = settingPath;
        }

        public string Delimiter { get; } = @"\";

        public string SubKeyName => string.Join(
            Delimiter,
            string.IsNullOrEmpty(_settingPath.ConfigurationName)
                ? _settingPath.Where((n, i) => i != SettingPath.ConfigurationNameIndex && i != _settingPath.Length - 1)
                : _settingPath.Where((n, i) => i != _settingPath.Length - 1));

        public string ValueName => _settingPath.Last();

        public override string ToString()
        {
            return string.Join(Delimiter, (IEnumerable<string>)_settingPath);
        }
    }
}
