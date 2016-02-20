using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig.Collections
{
    [DebuggerDisplay("NameKey = {NameKey}")]
    public class SettingKeyCollection : ReadOnlyCollection<SettingKey>
    {
        internal SettingKeyCollection(IList<SettingKey> settingKeys)
            : base(settingKeys)
        { }

        internal SettingKeyCollection(SettingKey nameKey, IEnumerable<SettingKey> otherKeys)
            : base(new[] { nameKey }.Concat(otherKeys).ToList())
        { }

        public NameKey NameKey => new NameKey(this.First());

        public IEnumerable<SettingKey> CustomKeys => this.Skip(1);
    }
}
