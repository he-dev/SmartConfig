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
            //var propertyName = property.Key;

            //var result =
            //    settings
            //    // get matching settings and items with the asterisk
            //    .Where(setting =>
            //        setting[propertyName].Equals(property.Value, StringComparison.OrdinalIgnoreCase) ||
            //        setting[propertyName].Equals(Wildcards.Asterisk))
            //    // sort settings so that the * is at the end
            //    .OrderByDescending(setting => setting[propertyName]);

            var result =
                settings
                    .Where(setting => setting[property.Key].Equals(property.Value, StringComparison.OrdinalIgnoreCase))
                    .Concat(settings.Where(setting => setting[property.Key].Equals(Wildcards.Asterisk)));

            return result;
        }

        public static IEnumerable<IIndexer> FilterByVersion(IEnumerable<IIndexer> settings, KeyValuePair<string, string> property)
        {
            var filtered = settings
                // get matching versions
                .Where(setting =>
                    !setting[property.Key].Equals(Wildcards.Asterisk) &&
                    SemanticVersion.Parse(setting[property.Key]) <= SemanticVersion.Parse(property.Value))
                // sort versions
                .OrderByDescending(setting => SemanticVersion.Parse(setting[property.Key]))
                // attach * at the end
                .Concat(settings.Where(setting => setting[property.Key].Equals(Wildcards.Asterisk)));

            // there can be only one version
            return filtered;
        }
    }
}
