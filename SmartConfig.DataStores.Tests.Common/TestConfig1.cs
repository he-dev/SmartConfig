using System.Collections.Generic;
using System.Linq;
using Reusable.Data.Annotations;
using SmartConfig.Data;
using SmartConfig.Data.Annotations;
// ReSharper disable InconsistentNaming

namespace SmartConfig.DataStores.Tests.Common
{
    [SmartConfig]
    public static class TestConfig1
    {
        public static string Utf8SettingDE { get; set; }

        public static string Utf8SettingPL { get; set; }

        [Optional]
        public static string OptionalStringSetting { get; set; } = "Default value";

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
            public static string StringSetting { get; set; } = "Ignored value";
        }
    }
}