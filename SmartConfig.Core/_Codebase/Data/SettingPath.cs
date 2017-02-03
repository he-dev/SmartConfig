using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Reusable.Fuse;

namespace SmartConfig.Data
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SettingPath : IReadOnlyCollection<string>
    {
        private const string DefaultDelimiter = ".";

        public SettingPath(IEnumerable<string> names, string key)
        {
            Names = names.Validate(nameof(names)).IsNotNull().IsTrue(x => x.Any()).Value.ToList();
            Key = key;
        }

        public SettingPath(IEnumerable<string> names) : this(names, null) { }

        public SettingPath(SettingPath urn) : this(urn, urn.Key) { }

        private IReadOnlyCollection<string> Names { get; }

        public string Delimiter { get; protected set; } = DefaultDelimiter;

        public string Namespace => string.Join(Delimiter, Names.Take(Count - 1));

        public string WeakName => Names.Last();

        public string WeakFullName => string.Join(Delimiter, Names);

        public string StrongName => string.IsNullOrEmpty(Key) ? WeakName : $"{WeakName}[{Key}]";

        public string StrongFullName => Names.Count > 1 ? $"{Namespace}{Delimiter}{StrongName}" : StrongName;

        public string Key { get; }

        public bool HasKey => !string.IsNullOrEmpty(Key);

        private string DebuggerDisplay => $"StrongFullName = \"{StrongFullName}\"";

        public int Count => Names.Count;

        public static SettingPath Parse(string value, string delimiter = DefaultDelimiter)
        {
            var names = value.Trim().Split(new[] { delimiter }, StringSplitOptions.None);

            // extract the key from the last name
            // https://regex101.com/r/qT2xX9/2
            var lastNameMatch = Regex.Match(names[names.Length - 1], @"(?<name>[a-z_][a-z0-9_]*)(\[(?<key>.+)\])?$", RegexOptions.IgnoreCase);
            names[names.Length - 1] = lastNameMatch.Groups["name"].Value;
            var key = lastNameMatch.Groups["key"].Value;

            return new SettingPath(names, key);
        }

        public bool IsLike(SettingPath path) => WeakFullName.Equals(path.WeakFullName, StringComparison.OrdinalIgnoreCase);

        public bool IsLike(string value) => IsLike(Parse(value));

#if DEBUG
        public override string ToString()
        {
            throw new InvalidOperationException($"Use {WeakFullName} instead.");
        }
#endif

        public static bool operator ==(SettingPath left, SettingPath right)
        {
            return
                !ReferenceEquals(left, null) &&
                !ReferenceEquals(right, null) &&
                left.StrongFullName.Equals(right.StrongFullName, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(SettingPath x, SettingPath y) => !(x == y);

        protected bool Equals(SettingPath other) => this == other;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SettingPath)obj);
        }

        public override int GetHashCode() => StrongFullName.GetHashCode();

        public IEnumerator<string> GetEnumerator() => Names.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
