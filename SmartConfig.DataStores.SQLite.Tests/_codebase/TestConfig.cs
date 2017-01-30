using System.Collections.Generic;
using Reusable.Data.Annotations;
using SmartConfig.Data.Annotations;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace SmartConfig.DataStores.SQLite.Tests
{
    [SmartConfig]
    public static class FullConfig1
    {
        public static string Utf8SettingDE { get; set; }

        public static string Utf8SettingPL { get; set; }

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

    [SmartConfig("thud")]
    public static class FullConfig2
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

    [SmartConfig("thudy", ConfigurationNameTarget.Tag)]
    public static class FullConfig3
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

    [SmartConfig("thudy")]
    public static class FullConfig4
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
