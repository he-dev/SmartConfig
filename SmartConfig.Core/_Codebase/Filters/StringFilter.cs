using System;
using System.Collections.Generic;
using System.Linq;
using SmartConfig.Data;

namespace SmartConfig.Filters
{
    /// <summary>
    /// Implements a string filter.
    /// </summary>
    public class StringFilter : ISettingFilter
    {
        public IEnumerable<IIndexable> Apply(IEnumerable<IIndexable> settings, KeyValuePair<string, object> custom)
        {
            var result = settings
                // get settings by custom key and asterisk
                .Where(setting =>
                    setting[custom.Key].Equals(custom.Value.ToString(), StringComparison.OrdinalIgnoreCase) ||
                    setting[custom.Key].Equals(Wildcards.Asterisk)
                )
                // sort settings with asterisk last
                .OrderBy(x => x[custom.Key], new WildcardComparer());

            return result;
        }

    }
}
