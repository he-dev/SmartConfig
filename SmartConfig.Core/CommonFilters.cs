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
    public static class CommonFilters
    {
        public static IEnumerable<IEnvironment> FilterByEnvironment(IEnumerable<IEnvironment> elements, string environment)
        {
            return elements.Where(e => e.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<IMachineName> FilterByMachineName(IEnumerable<IMachineName> elements, string environment)
        {
            return elements.Where(e => e.MachineName.Equals(environment, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<IUserName> FilterByUserName(IEnumerable<IUserName> elements, string environment)
        {
            return elements.Where(e => e.UserName.Equals(environment, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<ISemanticVersion> FilterBySemanticVersion(IEnumerable<ISemanticVersion> elements, string version)
        {
            var semVer = SemanticVersion.Parse(version);
            var result =
                elements
                // Get versions that are less or equal to current:
               .Where(e => SemanticVersion.Parse(e.SemanticVersion) <= semVer)
                // Sort by version:
               .OrderByDescending(e => SemanticVersion.Parse(e.SemanticVersion))
               .Take(1);
            return result;
        }
    }
}
