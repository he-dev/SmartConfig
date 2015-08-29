using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Implements popular filters.
    /// </summary>
    public static class Filters
    {
        public static IEnumerable<IIndexer> FilterByString(IEnumerable<IIndexer> elements, KeyValuePair<string, string> property)
        {
            var result =
                elements
                // first sort items by value
                .OrderByDescending(e => e[property.Key])
                // then either get the matching item or the one with the asterisk
                .Where(e =>
                    e[property.Key].Equals(property.Value, StringComparison.OrdinalIgnoreCase)
                    || e[property.Key].Equals(Wildcards.Asterisk));
            return result;
        }

        public static IEnumerable<IIndexer> FilterByVersion(IEnumerable<IIndexer> elements, KeyValuePair<string, string> property)
        {
            var versions =
                elements
                .Where(e => e[property.Key] != Wildcards.Asterisk)
                // get versions that are less or equal to current
                .Where(e => SemanticVersion.Parse(e[property.Key]) <= SemanticVersion.Parse(property.Value))
                // sort versions
                .OrderByDescending(e => SemanticVersion.Parse(e[property.Key]))
                .ToList();

            if (versions.Count == 0)
            {
                versions = elements.Where(e => e[property.Key] == Wildcards.Asterisk).ToList();
            }

            return versions.Take(1);
        }
    }
}
