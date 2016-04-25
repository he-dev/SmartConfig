using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartConfig.Filters;

namespace SmartConfig.DataStores.SQLite.Tests
{
    public class CustomTestSetting : BasicSetting
    {
        public CustomTestSetting() { }

        [SettingFilter(typeof(StringFilter))]
        public string Environment { get; set; }
    }
}
