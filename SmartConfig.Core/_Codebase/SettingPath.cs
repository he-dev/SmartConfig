using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SmartConfig
{
    /// <summary>
    /// Provides utility methods for creating configuration element names.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class SettingPath : IEnumerable<string>
    {
        internal const int ConfigurationNameIndex = 0;

        private readonly List<string> _path = new List<string>();

        protected SettingPath() { }

        public SettingPath(string configName, IEnumerable<string> path)
        {
            if (path == null) { throw new ArgumentNullException(nameof(path)); }
            if (!path.Any()) { throw new ArgumentException("Identifiers must not be empty."); }

            _path.Add(configName);
            _path.AddRange(path);
        }

        // supports path creation for unit tests
        internal SettingPath(string configName, params string[] path) : this(configName, (IEnumerable<string>)path) { }

        public string Delimiter { get; set; } = ".";

        public int Length => _path.Count;

        public bool ContainsConfigurationName => !string.IsNullOrEmpty(_path.FirstOrDefault());

        public string ConfigurationName => _path.FirstOrDefault();

        public IEnumerable<string> WithoutConfigurationName => _path.Skip(1);

        public override string ToString()
        {
            return string.Join(Delimiter, _path.Where(n => !string.IsNullOrEmpty(n)));
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _path.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
                x._path.SequenceEqual(y._path);
        }

        public static bool operator !=(SettingPath x, SettingPath y)
        {
            return !(x == y);
        }
    }
}
