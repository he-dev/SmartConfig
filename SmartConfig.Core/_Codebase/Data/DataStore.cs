using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartConfig.Data
{
    public abstract class DataStore
    {
        protected DataStore(IEnumerable<Type> supportedTypes)
        {
            SupportedTypes = supportedTypes.ToList();
        }

        public IReadOnlyCollection<Type> SupportedTypes { get; }

        public abstract IEnumerable<Setting> GetSettings(Setting setting);

        public abstract int SaveSettings(IEnumerable<Setting> settings);

        public int SaveSetting(Setting setting) => SaveSettings(new[] { setting });
    }
}
