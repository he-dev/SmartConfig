using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig.Collections
{
    public class SettingKeyNameReadOnlyCollection : ReadOnlyCollection<string>
    {
        private SettingKeyNameReadOnlyCollection(IList<string> list) : base(list) { }

        public IReadOnlyCollection<string> CustomKeyNames => new ReadOnlyCollection<string>(this.Where(x => x != Setting.DefaultKeyName).ToList());

        internal static SettingKeyNameReadOnlyCollection Create<TSetting>() where TSetting : Setting
        {
            var keyNames = new List<string> { Setting.DefaultKeyName };

            var settingType = typeof(TSetting);

            var isCustomType = settingType != typeof(Setting);
            if (!isCustomType)
            {
                return new SettingKeyNameReadOnlyCollection(keyNames);
            }

            var customPropertyNames = settingType
                    .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => p.Name)
                    .OrderBy(n => n);
            keyNames.AddRange(customPropertyNames);

            return new SettingKeyNameReadOnlyCollection(keyNames);
        }
    }
}
