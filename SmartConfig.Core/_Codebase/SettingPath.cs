using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SettingPath : IReadOnlyCollection<string>
    {
        private const string DefaultDelimiter = ".";

        public SettingPath(IList<string> names, string valueKey = null)
        {
            Names = new List<string>(names.Validate(nameof(names)).IsNotNull().IsTrue(x => x.Any()).Argument);
            ValueKey = valueKey;
        }

        public SettingPath(SettingPath path, string valueKey = null)
        {
            Names = path.Names;
            ValueKey = valueKey ?? path.ValueKey;
        }

        private IReadOnlyCollection<string> Names { get; }

        public string Delimiter { get; protected set; } = DefaultDelimiter;

        public string SettingNamespace => string.Join(Delimiter, Names.Take(Count - 1));

        public string SettingName => Names.Last();

        public string SettingNameEx => string.IsNullOrEmpty(ValueKey) ? SettingName : $"{SettingName}[{ValueKey}]";

        public string FullName => string.Join(Delimiter, Names);

        public string FullNameEx =>  Names.Count > 1 ? $"{SettingNamespace}{Delimiter}{SettingNameEx}" : SettingNameEx;

        public string ValueKey { get; }

        private string DebuggerDisplay => FullNameEx;

        public int Count => Names.Count;

        public static SettingPath Parse(string value, string delimiter = DefaultDelimiter)
        {
            var names = value.Trim().Split(new[] { delimiter }, StringSplitOptions.None);

            // extract the key from the last name
            // https://regex101.com/r/qT2xX9/2
            var lastNameMatch = Regex.Match(names[names.Length - 1], @"(?<name>[a-z_][a-z0-9_]*)(\[(?<key>.+)\])?$", RegexOptions.IgnoreCase);
            names[names.Length - 1] = lastNameMatch.Groups["name"].Value;
            var valueKey = lastNameMatch.Groups["key"].Value;

            return new SettingPath(names, valueKey);
        }      

        public bool IsLike(SettingPath path)
        {
            return FullName.Equals(path.FullName, StringComparison.OrdinalIgnoreCase);
        }

#if DEBUG
        public override string ToString()
        {
            throw new InvalidOperationException($"Use {FullName} instead.");
        }
#endif

        public static explicit operator string(SettingPath settingPath) => settingPath.FullNameEx;

        public static bool operator ==(SettingPath x, SettingPath y)
        {
            return
                !ReferenceEquals(x, null) &&
                !ReferenceEquals(y, null) &&
                x.FullNameEx.Equals(y.FullNameEx, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(SettingPath x, SettingPath y)
        {
            return !(x == y);
        }

        public static implicit operator SettingPath(string name)
        {
            return Parse(name);
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

        public override int GetHashCode() => FullNameEx.GetHashCode();

        public IEnumerator<string> GetEnumerator() => Names.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
