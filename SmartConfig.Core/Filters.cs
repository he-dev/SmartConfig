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
        public static IEnumerable<T> FilterByString<T>(IEnumerable<T> elements, KeyValuePair<string, string> property) where T : Setting
        {
            var result =
                elements
                // first sort items by value
                .OrderByDescending(e => e.GetStringDelegates[property.Key]())
                // then either get the matching item or the one with the asterisk
                .Where(e =>
                    e.GetStringDelegates[property.Key]().Equals(property.Value, StringComparison.OrdinalIgnoreCase)
                    || e.GetStringDelegates[property.Key]().Equals(Wildcards.Asterisk));
            return result;
        }

        public static IEnumerable<T> FilterByVersion<T>(IEnumerable<T> elements, KeyValuePair<string, string> property) where T : Setting
        {
            var versions =
                elements
                .Where(e => e.GetStringDelegates[property.Key]() != Wildcards.Asterisk)
                // get versions that are less or equal to current
                .Where(e => SemanticVersion.Parse(e.GetStringDelegates[property.Key]()) <= SemanticVersion.Parse(property.Value))
                // sort versions
                .OrderByDescending(e => SemanticVersion.Parse(e.GetStringDelegates[property.Key]()))
                .ToList();

            if (versions.Count == 0)
            {
                versions = elements.Where(e => e.GetStringDelegates[property.Key]() == Wildcards.Asterisk).ToList();
            }

            return versions.Take(1);
        }
    }
}
