using SmartConfig.Data;
using SmartConfig.Filters;

namespace SmartConfig.Tests
{
    public class TestSetting : Setting
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

        [Filter(typeof(StringFilter))]
        public string Environment { get; set; }

        [Filter(typeof(VersionFilter))]
        public string Version { get; set; }
    }
}
