using SmartConfig.Data;
using SmartConfig.Filters;

namespace SmartConfig.Core.Tests
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

        [SettingFilter(typeof(StringFilter))]
        public string Environment { get; set; }

        [SettingFilter(typeof(VersionFilter))]
        public string Version { get; set; }
    }
}
