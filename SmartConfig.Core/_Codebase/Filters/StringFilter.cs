using System;
using System.Collections.Generic;
using System.Linq;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig.Filters
{
    /// <summary>
    /// Implements popular filters.
    /// </summary>
    public class StringFilter : ISettingFilter
    {
        public IEnumerable<IIndexer> FilterSettings(IEnumerable<IIndexer> settings, SettingKey key)
        {
            var result = settings
                    .Where(setting => setting[key.Name].Equals(key.Value.ToString(), StringComparison.OrdinalIgnoreCase))
                    .Concat(settings.Where(setting => setting[key.Name].Equals(Wildcards.Asterisk)));

            return result;
        }        
    }
}
