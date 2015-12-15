using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig.Collections
{
    public class SettingKeyReadOnlyCollection : ReadOnlyCollection<SettingKey>
    {
        internal SettingKeyReadOnlyCollection(IList<SettingKey> settingKeys) : base(settingKeys) { }

        internal SettingKeyReadOnlyCollection(params SettingKey[] settingKeys) : base(settingKeys) { }

        public SettingKey DefaultKey => this.First();

        public IEnumerable<SettingKey> CustomKeys => this.Skip(1);
    }
}
