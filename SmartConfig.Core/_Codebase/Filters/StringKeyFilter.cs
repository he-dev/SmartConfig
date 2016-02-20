using System;
using System.Collections.Generic;
using System.Linq;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig.Filters
{
    /// <summary>
    /// Implements a string filter.
    /// </summary>
    public class StringKeyFilter : IKeyFilter
    {
        public IEnumerable<IIndexable> Apply(IEnumerable<IIndexable> settings, SettingKey key)
        {
            var result = settings
                    .Where(setting => setting[key.Name].Equals(key.Value.ToString(), StringComparison.OrdinalIgnoreCase))
                    .Concat(settings.Where(setting => setting[key.Name].Equals(Wildcards.Asterisk)));

            return result;
        }        
    }
}
