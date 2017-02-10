using Reusable.Data.Annotations;
using SmartConfig.Data.Annotations;

namespace SmartConfig.DataStores.Tests.Common
{
    [SmartConfig]
    [SettingName("thudy")]
    public static class TestConfig4
    {
        public static string StringSetting { get; set; }

        [Optional]
        public static string OptionalStringSetting { get; set; } = "Waldo";

        public static class NestedConfig
        {
            public static string StringSetting { get; set; }
        }

        [Ignore]
        public static class IgnoredConfig
        {
            public static string StringSetting { get; set; } = "Grault";
        }
    }
}