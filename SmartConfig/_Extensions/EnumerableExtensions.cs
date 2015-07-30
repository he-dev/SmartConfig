using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    public static class EnumerableExtensions
    {
        public static void Check<T>(this IEnumerable<ValueConstraintAttribute> contraints, Action<T> checkAction) where T : ValueConstraintAttribute
        {
            var constraint = contraints.OfType<T>().SingleOrDefault();
            if (constraint != null)
            {
                checkAction(constraint);
            }
        }

        #region ConfigElement Filters

        public static IEnumerable<ConfigElement> FilterByEnvironment(this IEnumerable<ConfigElement> elements, string environment)
        {
            return elements.Where(e => e.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<ConfigElement> FilterBySemanticVersion(this IEnumerable<ConfigElement> elements, string version)
        {
            var semVer = SemanticVersion.Parse(version);
            return elements
                // Get versions that are less or equal to current:
                .Where(e => SemanticVersion.Parse(e.Version) <= semVer)
                // Sort by version:
                .OrderByDescending(e => SemanticVersion.Parse(e.Version))
                .Take(1);
        }

        #endregion
    }
}
