using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SmartConfig.Paths
{
    /// <summary>
    /// Provides utility methods for creating configuration element names.
    /// </summary>
    [DebuggerDisplay("{this.ToString()}")]
    public class SettingPath : IEnumerable<string>
    {
        internal const int ConfigurationNameIndex = 0;

        private readonly List<string> _names = new List<string>();

        protected SettingPath() { }

        public SettingPath(string configName, IEnumerable<string> path)
        {
            _names.Add(configName);

            if (!path.Any()) { throw new ArgumentException("Path must not be empty", nameof(path)); }

            _names.AddRange(path);
        }

        // supports path creation for unit tests
        internal SettingPath(string configName, params string[] path) : this(configName, (IEnumerable<string>)path) { }

        public string Delimiter { get; set; } = ".";

        public int Length => _names.Count;

        public string ConfigurationName => _names.FirstOrDefault();

        //public IEnumerable<string> PathWithoutConfigurationName => _names.Skip(1);

        public override string ToString()
        {
            return string.Join(Delimiter, _names.Where(n => !string.IsNullOrEmpty(n)));
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _names.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator string(SettingPath settingPath)
        {
            return settingPath.ToString();
        }
    }
}
