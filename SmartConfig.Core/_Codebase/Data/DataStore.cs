using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartConfig.Data
{
    // Base class for all data stores. Requires supported-types.
    public abstract class DataStore
    {
        protected DataStore(IEnumerable<Type> supportedTypes)
        {
            SupportedTypes = supportedTypes.ToList();
        }

        public IReadOnlyCollection<Type> SupportedTypes { get; }

        public abstract IEnumerable<Setting> ReadSettings(Setting setting);

        public void WriteSettings(IEnumerable<Setting> settings)=> WriteSettings(settings.GroupBy(x => x, new WeakSettingComparer()).ToList());

        protected abstract void WriteSettings(ICollection<IGrouping<Setting, Setting>> settings);
    }
}
