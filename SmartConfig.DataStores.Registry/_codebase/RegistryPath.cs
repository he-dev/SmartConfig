using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;

namespace SmartConfig.DataStores.Registry
{
    public class RegistryPath
    {
        private readonly SettingPath _settingPath;

        public RegistryPath(SettingPath settingPath)
        {
            _settingPath = settingPath;
        }

        public string Delimiter { get; } = @"\";

        //public string SubKeyName => string.Empty;
        public string SubKeyName => string.Join(
            Delimiter,
            _settingPath.ContainsConfigurationName
                ? _settingPath.Take(_settingPath.Count - 1)
                : _settingPath.WithoutConfigurationName.Take(_settingPath.Count - 2));


        public string ValueName => _settingPath.Last();

        public override string ToString()
        {
            return string.Join(Delimiter, (IEnumerable<string>)_settingPath);
        }
    }
}
