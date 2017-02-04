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

        public abstract IEnumerable<Setting> GetSettings(Setting setting);

        public abstract int SaveSettings(IEnumerable<Setting> settings);
    }
}
