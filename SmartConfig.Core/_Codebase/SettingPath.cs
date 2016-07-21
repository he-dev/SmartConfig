using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SettingPath : ReadOnlyCollection<string>
    {
        internal SettingPath(IList<string> names) : base(names)
        {
            names.Validate(nameof(names)).IsNotNull().IsTrue(x => x.Any());
        }

        internal SettingPath(params string[] names) : this(names.ToList()) { }

        public string Delimiter { get; set; } = ".";

        public bool ContainsConfigurationName => !string.IsNullOrEmpty(this.FirstOrDefault());

        public string ConfigurationName => this.FirstOrDefault();

        public IEnumerable<string> WithoutConfigurationName => this.Skip(1);

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
