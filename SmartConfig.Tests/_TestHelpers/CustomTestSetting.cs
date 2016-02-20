using SmartConfig.Data;
using SmartConfig.Filters;

namespace SmartConfig.Tests
{
    public class CustomTestSetting : BasicSetting
    {
        public CustomTestSetting() { }

        public CustomTestSetting(string values)
        {
            var columns = values.Split('|');
            Environment = columns[0];
            Version = columns[1];
            Name = columns[2];
            Value = columns[3];
        }

        [KeyFilter(typeof(StringKeyFilter))]
        public string Environment { get; set; }

        [KeyFilter(typeof(VersionKeyFilter))]
        public string Version { get; set; }
    }
}
