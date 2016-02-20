using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig.Collections
{
    public class SettingKeyNameCollection<TSetting> : ReadOnlyCollection<string> where TSetting : BasicSetting
    {
        public SettingKeyNameCollection()
            : base(Reflector.GetSettingKeyNames<TSetting>().ToList())
        {
        }

        public IReadOnlyCollection<string> CustomKeyNames
            => new ReadOnlyCollection<string>(this.Skip(1).ToList());
    }
}
