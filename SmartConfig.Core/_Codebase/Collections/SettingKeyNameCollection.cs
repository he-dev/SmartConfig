using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig.Collections
{
    public class SettingKeyNameCollection : ReadOnlyCollection<string>
    {
        public SettingKeyNameCollection(IList<string> settingKeyNames)
            : base(settingKeyNames)
        {
        }

        public IReadOnlyCollection<string> CustomKeyNames => new ReadOnlyCollection<string>(this.Skip(1).ToList());

        public static SettingKeyNameCollection Create<TSetting>() where TSetting : BasicSetting
        {
            var settingKeyNames = new List<string> { nameof(BasicSetting.Name) };

            var settingType = typeof(TSetting);

            var isBasicSettingType = settingType == typeof(BasicSetting);
            if (isBasicSettingType)
            {
                return new SettingKeyNameCollection(settingKeyNames);
            }

            var additionalSettingKeyNames = settingType
                .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .Select(p => p.Name)
                .OrderBy(n => n);
            settingKeyNames.AddRange(additionalSettingKeyNames);
            return new SettingKeyNameCollection(settingKeyNames);
        }
    }
}
