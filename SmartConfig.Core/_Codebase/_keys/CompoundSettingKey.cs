using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig
{
    [DebuggerDisplay("NameKey = {NameKey}")]
    public class CompoundSettingKey : ReadOnlyCollection<SimpleSettingKey>
    {
        internal CompoundSettingKey(IList<SimpleSettingKey> simpleKeys)
            : base(simpleKeys)
        { }

        internal CompoundSettingKey(SimpleSettingKey nameKey, IEnumerable<SimpleSettingKey> otherKeys)
            : base(new[] { nameKey }.Concat(otherKeys).ToList())
        { }

        public NameKey NameKey => new NameKey(this.First());

        public IEnumerable<SimpleSettingKey> CustomKeys => this.Skip(1);
    }
}
