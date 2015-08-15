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
        public static IEnumerable<T> FilterByString<T>(IEnumerable<T> elements, KeyValuePair<string, string> property) where T : ConfigElement
        {
            var result = elements.Where(e => e.GetStringDelegates[property.Key]().Equals(property.Value, StringComparison.OrdinalIgnoreCase));
            if (!result.Any())
            {
                result = elements.Where(e => e.GetStringDelegates[property.Key]().Equals(Wildcards.Asterisk));
            }
            return result;
        }        

        public static IEnumerable<T> FilterByVersion<T>(IEnumerable<T> elements, KeyValuePair<string, string> property) where T : ConfigElement
        {             
            var semVer = SemanticVersion.Parse(property.Value);
            var result =
                elements
               // Get versions that are less or equal to current:
               .Where(e => SemanticVersion.Parse(e.GetStringDelegates[property.Key]()) <= semVer)
               // Sort by version:
               .OrderByDescending(e => SemanticVersion.Parse(e.GetStringDelegates[property.Key]()))
               .Take(1);
            return result;
        }
    }
}
