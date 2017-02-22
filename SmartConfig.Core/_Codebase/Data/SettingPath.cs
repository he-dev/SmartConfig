using Reusable.Fuse;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmartConfig.Data
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class SettingPath : IReadOnlyCollection<string>
    {
        private const string DefaultDelimiter = ".";

        public SettingPath(IEnumerable<string> names)
        {
            Names = names.Validate(nameof(names)).IsNotNull().IsTrue(x => x.Any()).Value.ToList();
        }

        public SettingPath(params string[] names) : this((IEnumerable<string>)names) { }

        public SettingPath(SettingPath path) : this(path.Names)
        {
            Key = path.Key;
        }

        private IReadOnlyCollection<string> Names { get; }

        public string Delimiter { get; protected set; } = DefaultDelimiter;

        public string Namespace => string.Join(Delimiter, Names.Take(Count - 1));

        public string WeakName => Names.Last();

        public string WeakFullName => string.Join(Delimiter, Names);

        public string StrongName => string.IsNullOrEmpty(Key) ? WeakName : $"{WeakName}[{Key}]";

        public string StrongFullName => Names.Count > 1 ? $"{Namespace}{Delimiter}{StrongName}" : StrongName;

        public string Key { get; set; }

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

            return new SettingPath(names)
            {
                Key = key                
            };
        }

#if DEBUG
        public override string ToString()
        {
            throw new InvalidOperationException($"{nameof(ToString)} was used. Did you mean {nameof(WeakFullName)} instead?");
        }
#endif       

        public override int GetHashCode() => StrongFullName.GetHashCode();

        public IEnumerator<string> GetEnumerator() => Names.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class SettingPathExtensions
    {
        private static readonly WeakSettingPathComparer WeakSettingPathComparer = new WeakSettingPathComparer();

        public static bool IsLike(this SettingPath x, SettingPath y)
        {
            return WeakSettingPathComparer.Equals(x, y);
        }
    }
}
