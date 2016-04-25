using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartConfig.Filters;

namespace SmartConfig.DataStores.XmlFile.Tests
{
    public class CustomTestSetting : BasicSetting
    {
        [SettingFilter(typeof(StringFilter))]
        public string Environment { get; set; }

        [SettingFilter(typeof(VersionFilter))]
        public string Version { get; set; }
    }
}
