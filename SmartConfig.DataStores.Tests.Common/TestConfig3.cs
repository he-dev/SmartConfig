using System.Collections.Generic;
using Reusable.Data.Annotations;
using SmartConfig.Data.Annotations;

namespace SmartConfig.DataStores.Tests.Common
{
    [SmartConfig(SettingNameTarget.Tag)]
    [SettingName("thudy")]
    public static class TestConfig3
    {
        public static string StringSetting { get; set; }

        [Optional]
        public static string OptionalStringSetting { get; set; } = "Waldo";

        [Itemized]
        public static int[] ArraySetting { get; set; }

        [Itemized]
        public static Dictionary<string, int> DictionarySetting { get; set; }

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