using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig.Collections
{
    [DebuggerDisplay("DefaultKey = {DefaultKey}")]
    public class SettingKeyCollection : ReadOnlyCollection<SettingKey>
    {
        internal SettingKeyCollection(IList<SettingKey> settingKeys)
            : base(settingKeys)
        { }

        internal SettingKeyCollection(SettingKey defaultKey, IEnumerable<SettingKey> customKeys)
            : base(new[] { defaultKey }.Concat(customKeys).ToList())
        { }

        public SettingKey DefaultKey => this.First();

        public IEnumerable<SettingKey> CustomKeys => this.Skip(1);
    }
}
