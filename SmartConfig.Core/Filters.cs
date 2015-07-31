using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    public static class Filters
    {
        #region ConfigElement Filters

        public static IEnumerable<IEnvironment> FilterByEnvironment(IEnumerable<IEnvironment> elements, string environment)
        {
            return elements.Where(e => e.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<IVersion> FilterBySemanticVersion(IEnumerable<IVersion> elements, string version)
        {
            var semVer = SemanticVersion.Parse(version);
            var result =
                elements
                // Get versions that are less or equal to current:
               .Where(e => SemanticVersion.Parse(e.Version) <= semVer)
                // Sort by version:
               .OrderByDescending(e => SemanticVersion.Parse(e.Version))
               .Take(1);
            return result;
        }

        #endregion
    }
}
