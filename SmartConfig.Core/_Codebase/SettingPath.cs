using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace SmartConfig
{
    /// <summary>
    /// Provides utility methods for creating configuration element names.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SettingPath : ReadOnlyCollection<string>
    {
        internal SettingPath(IList<string> names) : base(names)
        {
            if (names == null) { throw new ArgumentNullException(nameof(names)); }
            if (!names.Any()) { throw new ArgumentException("There must be at least one name."); }
        }

        public string Delimiter { get; set; } = ".";

        public bool ContainsConfigurationName => !string.IsNullOrEmpty(this.FirstOrDefault());

        public string ConfigurationName => this.FirstOrDefault();

        public IEnumerable<string> WithoutConfigurationName => this.Skip(1);

        internal static SettingPath Create(string configName, IEnumerable<string> names)
        {
            if (names == null) { throw new ArgumentNullException(nameof(names)); }

            return new SettingPath(new[] { configName }.Concat(names).ToList());
        }

        internal static SettingPath Create(string configName,params string[] names)
        {
            return Create(configName, (IEnumerable<string>) names);
        }

        private string DebuggerDisplay => ToString();

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
