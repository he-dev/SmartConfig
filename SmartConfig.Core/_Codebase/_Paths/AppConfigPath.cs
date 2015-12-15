using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartConfig
{
    /// <summary>
    /// Provides utility methods for creating configuration element names.
    /// </summary>
    public class AppConfigPath : SettingPath
    {
        private readonly SettingPath _settingPath;

        private readonly int _sectionNameIndex;

        public AppConfigPath(SettingPath settingPath, IEnumerable<string> sectionNames)
        {
            _settingPath = settingPath;
            _sectionNameIndex = _settingPath.Names.Select((n, i) => new { n, i }).First(x => sectionNames.Contains(x.n)).i;
        }

        public override IReadOnlyCollection<string> Names => _settingPath.Names;

        public string SectionName => _settingPath.Names.ElementAt(_sectionNameIndex);

        public override string ToString()
        {
            return string.Join(Delimiter, _settingPath.Names.Where((n, i) => i != _sectionNameIndex));
        }
    }
}
