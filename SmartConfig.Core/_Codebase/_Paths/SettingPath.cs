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
    [DebuggerDisplay("{this.ToString()}")]
    public class SettingPath : IEnumerable<string>
    {
        private readonly List<string> _names = new List<string>();

        protected SettingPath() { }

        public SettingPath(string configName, IEnumerable<string> path)
        {
            if (!string.IsNullOrEmpty(configName))
            {
                ContainsConfigName = true;
                _names.Add(configName);
            }
            _names.AddRange(path);
        }

        // supports path creation for unit tests
        internal SettingPath(string configName, params string[] path) : this(configName, (IEnumerable<string>)path) { }

        public string Delimiter { get; set; } = ".";

        public bool ContainsConfigName { get; }

        public int Length => _names.Count;

        public override string ToString()
        {
            return string.Join(Delimiter, _names);
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
