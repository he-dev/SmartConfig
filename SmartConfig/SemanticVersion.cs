using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Provides basic functionality for semantic versioning.
    /// </summary>
    public class SemanticVersion : IComparable<SemanticVersion>
    {
        public static readonly string Pattern = @"v?(?<Major>\d+)\.(?<Minor>\d+)\.(?<Patch>\d+)";

        public static SemanticVersion Parse(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                return null;
            }

            var match = Regex.Match(version, Pattern);
            if (!match.Success)
            {
                return null;
            }

            return new SemanticVersion()
            {
                Major = int.Parse(match.Groups["Major"].Value),
                Minor = int.Parse(match.Groups["Minor"].Value),
                Patch = int.Parse(match.Groups["Patch"].Value)
            };
        }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Patch { get; set; }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var semVer = obj as SemanticVersion;
            if ((object)semVer == null)
            {
                return false;
            }

            return ToString() == semVer.ToString();
        }

        public int CompareTo(SemanticVersion other)
        {
            if (object.Equals(other, null))
            {
                return 1;
            }

            var lessThenZero =
                Major < other.Major
                || (Major == other.Major && Minor < other.Minor)
                || (Major == other.Major && Minor == other.Minor && Patch < other.Patch);

            if (lessThenZero)
            {
                return -1;
            }

            var zero = Major == other.Major && Minor == other.Minor && Patch == other.Patch;
            if (zero)
            {
                return 0;
            }

            var greaterThenZero =
                Major > other.Major
                || (Major == other.Major && Minor > other.Minor)
                || (Major == other.Major && Minor == other.Minor && Patch > other.Patch);

            if (greaterThenZero)
            {
                return 1;
            }

            throw new InvalidOperationException("Invalid version.");
        }

        public static bool operator <(SemanticVersion semVer1, SemanticVersion semVer2)
        {
            return semVer1.CompareTo(semVer2) < 0;
        }

        public static bool operator ==(SemanticVersion semVer1, SemanticVersion semVer2)
        {
            if (object.ReferenceEquals(semVer1, semVer2))
            {
                return true;
            }

            if ((object)semVer1 == null || (object)semVer2 == null)
            {
                return false;
            }

            return semVer1.CompareTo(semVer2) == 0;
        }

        public static bool operator !=(SemanticVersion semVer1, SemanticVersion semVer2)
        {
            return !(semVer1 == semVer2);
        }

        public static bool operator >(SemanticVersion semVer1, SemanticVersion semVer2)
        {
            return semVer1.CompareTo(semVer2) > 0;
        }

        public static bool operator <=(SemanticVersion semVer1, SemanticVersion semVer2)
        {
            return semVer1 < semVer2 || semVer1 == semVer2;
        }

        public static bool operator >=(SemanticVersion semVer1, SemanticVersion semVer2)
        {
            return semVer1 > semVer2 || semVer1 == semVer2;
        }
    }
}
