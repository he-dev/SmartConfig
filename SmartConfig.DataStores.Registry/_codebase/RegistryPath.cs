using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmartConfig.DataStores.Registry
{
    public class RegistryPath : ReadOnlyCollection<string>
    {
        private readonly SettingPath _settingPath;

        public RegistryPath(SettingPath settingPath) : base(settingPath)
        {
            _settingPath = settingPath;
        }

        public string Delimiter { get; } = @"\";

        public string SubKeyName => string.Join(
            Delimiter,
            _settingPath
                // the last element is the name of the value
                .Take(_settingPath.Count - 1)
                // configuraiton name may be null
                .Where(x => !string.IsNullOrEmpty(x)));

        // value is the last element
        public string ValueName => _settingPath.Last();

        public override string ToString() => string.Join(Delimiter, (IEnumerable<string>) _settingPath);
    }
}
