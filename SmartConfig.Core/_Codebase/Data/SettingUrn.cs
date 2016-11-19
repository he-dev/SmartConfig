using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Reusable;
using Reusable.Validations;

namespace SmartConfig.Data
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SettingUrn : IReadOnlyCollection<string>
    {
        private const string DefaultDelimiter = ".";

        public SettingUrn(IEnumerable<string> names, string key)
        {
            Names = names.Validate(nameof(names)).IsNotNull().IsTrue(x => x.Any()).Value.ToList();
            Key = key;
        }

        public SettingUrn(IEnumerable<string> names) : this(names, null) { }

        public SettingUrn(SettingUrn urn) : this(urn, urn.Key) { }

        private IReadOnlyCollection<string> Names { get; }

        public string Delimiter { get; protected set; } = DefaultDelimiter;

        public string Namespace => string.Join(Delimiter, Names.Take(Count - 1));

        public string Name => Names.Last();

        public string NameWithKey => string.IsNullOrEmpty(Key) ? Name : $"{Name}[{Key}]";

        public string FullName => string.Join(Delimiter, Names);

        public string FullNameWithKey => Names.Count > 1 ? $"{Namespace}{Delimiter}{NameWithKey}" : NameWithKey;

        public string Key { get; }

        private string DebuggerDisplay => FullNameWithKey;

        public int Count => Names.Count;

        public static SettingUrn Parse(string value, string delimiter = DefaultDelimiter)
        {
            var names = value.Trim().Split(new[] { delimiter }, StringSplitOptions.None);

            // extract the key from the last name
            // https://regex101.com/r/qT2xX9/2
            var lastNameMatch = Regex.Match(names[names.Length - 1], @"(?<name>[a-z_][a-z0-9_]*)(\[(?<key>.+)\])?$", RegexOptions.IgnoreCase);
            names[names.Length - 1] = lastNameMatch.Groups["name"].Value;
            var key = lastNameMatch.Groups["key"].Value;

            return new SettingUrn(names, key);
        }

        public bool IsLike(SettingUrn path)
        {
            return FullName.Equals(path.FullName, StringComparison.OrdinalIgnoreCase);
        }


#if DEBUG
        public override string ToString()
        {
            throw new InvalidOperationException($"Use {FullName} instead.");
        }
#endif

        public static explicit operator string(SettingUrn settingPath) => settingPath.FullNameWithKey;

        public static bool operator ==(SettingUrn left, SettingUrn right)
        {
            return
                !ReferenceEquals(left, null) &&
                !ReferenceEquals(right, null) &&
                left.FullNameWithKey.Equals(right.FullNameWithKey, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(SettingUrn x, SettingUrn y)
        {
            return !(x == y);
        }

        public static implicit operator SettingUrn(string name)
        {
            return Parse(name);
        }

        protected bool Equals(SettingUrn other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SettingUrn)obj);
        }

        public override int GetHashCode() => FullNameWithKey.GetHashCode();

        public IEnumerator<string> GetEnumerator() => Names.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
