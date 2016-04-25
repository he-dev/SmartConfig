using System.Collections.Generic;
using System.Linq;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig.Filters
{
    /// <summary>
    /// Implements popular filters.
    /// </summary>
    public class VersionFilter : ISettingFilter
    {
        public IEnumerable<IIndexable> Apply(IEnumerable<IIndexable> settings, KeyValuePair<string, object> custom)
        {
            var result = settings
                // get versions and asterisk
                .Where(setting =>
                    setting[custom.Key].Equals(Wildcards.Asterisk) ||
                    SemanticVersion.Parse(setting[custom.Key]) <= SemanticVersion.Parse(custom.Value.ToString())
                )
                // sort versions desc with the asterisk last
                .OrderByDescending(setting => setting[custom.Key] == Wildcards.Asterisk ? null : SemanticVersion.Parse(setting[custom.Key]));

            return result;
        }
    }
}
