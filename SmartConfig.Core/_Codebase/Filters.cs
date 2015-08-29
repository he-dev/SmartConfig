using System;
using System.Collections.Generic;
using System.Linq;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Implements popular filters.
    /// </summary>
    public static class Filters
    {
        public static IEnumerable<IIndexer> FilterByString(IEnumerable<IIndexer> settings, KeyValuePair<string, string> property)
        {
            var propertyName = property.Key;

            var result =
                settings
                // get matching settings and items with the asterisk
                .Where(setting =>
                    setting[propertyName].Equals(property.Value, StringComparison.OrdinalIgnoreCase) ||
                    setting[propertyName].Equals(Wildcards.Asterisk))
                // sort settings so that the * is at the end
                .OrderByDescending(setting => setting[propertyName]);

            return result;
        }

        public static IEnumerable<IIndexer> FilterByVersion(IEnumerable<IIndexer> settings, KeyValuePair<string, string> property)
        {
            var propertyName = property.Key;

            var filtered = settings
                // get matching versions
                .Where(setting =>
                    !setting[propertyName].Equals(Wildcards.Asterisk) &&
                    SemanticVersion.Parse(setting[propertyName]) <= SemanticVersion.Parse(property.Value))
                // sort versions
                .OrderByDescending(setting => SemanticVersion.Parse(setting[propertyName]))
                .AsEnumerable();
            
            // attach * at the end
            filtered = filtered.Concat(settings.Where(setting => setting[propertyName].Equals(Wildcards.Asterisk)));

            // there can be only one version
            return filtered.Take(1);
        }
    }
}
