using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace SmartConfig
{
    /// <summary>
    /// Provides utility methods for creating configuration element names.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class SettingPath : ReadOnlyCollection<string>
    {
        protected SettingPath(IList<string> list) : base(list) { }

        public SettingPath(string configName, IEnumerable<string> path) : this(new[] { configName }.Concat(path).ToList())
        {
            if (path == null) { throw new ArgumentNullException(nameof(path)); }
            if (!path.Any()) { throw new ArgumentException("Identifiers must not be empty."); }
        }

        // provides easy path creation for unit tests
        internal SettingPath(string configName, params string[] path) : this(new[] { configName }.Concat(path).ToList()) { }

        public string Delimiter { get; set; } = ".";

        public bool ContainsConfigurationName => !string.IsNullOrEmpty(this.FirstOrDefault());

        public string ConfigurationName => this.FirstOrDefault();

        public IEnumerable<string> WithoutConfigurationName => this.Skip(1);

        public override string ToString()
        {
            return string.Join(Delimiter, this.Where(n => !string.IsNullOrEmpty(n)));
        }

        public static implicit operator string(SettingPath settingPath)
        {
            return settingPath.ToString();
        }

        public static bool operator ==(SettingPath x, SettingPath y)
        {
            return
                !ReferenceEquals(x, null) &&
                !ReferenceEquals(y, null) &&
                x.SequenceEqual(y);
        }

        public static bool operator !=(SettingPath x, SettingPath y)
        {
            return !(x == y);
        }

        protected bool Equals(SettingPath other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SettingPath)obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
