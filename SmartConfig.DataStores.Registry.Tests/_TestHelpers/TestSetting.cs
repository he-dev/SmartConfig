using SmartConfig.Data;
using SmartConfig.Filters;

namespace SmartConfig.DataStores.Registry.Tests
{
    public class TestSetting : BasicSetting
    {
        public TestSetting() { }

        public TestSetting(string values)
        {
            var columns = values.Split('|');
            Environment = columns[0];
            Version = columns[1];
            Name = columns[2];
            Value = columns[3];
        }

        [SettingFilter(typeof(StringFilter))]
        public string Environment { get; set; }

        [SettingFilter(typeof(VersionFilter))]
        public string Version { get; set; }
    }
}
