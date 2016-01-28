using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig.Collections
{
    public class SettingKeyNameCollection : ReadOnlyCollection<string>
    {
        private SettingKeyNameCollection(IList<string> list) : base(list) { }

        public IReadOnlyCollection<string> CustomKeyNames => new ReadOnlyCollection<string>(this.Where(x => x != Setting.DefaultKeyName).ToList());

        internal static SettingKeyNameCollection Create<TSetting>() where TSetting : Setting
        {
            var keyNames = new List<string> { Setting.DefaultKeyName };

            var settingType = typeof(TSetting);

            var isCustomType = settingType != typeof(Setting);
            if (!isCustomType)
            {
                return new SettingKeyNameCollection(keyNames);
            }

            var customPropertyNames = settingType
                    .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => p.Name)
                    .OrderBy(n => n);
            keyNames.AddRange(customPropertyNames);

            return new SettingKeyNameCollection(keyNames);
        }
    }
}
