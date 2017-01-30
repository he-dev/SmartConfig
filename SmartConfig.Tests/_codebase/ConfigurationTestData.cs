using System;
using System.Collections.Generic;
using System.Drawing;
using Reusable.Converters;
using Reusable.Data.Annotations;
using SmartConfig.Data.Annotations;

namespace SmartConfig.Core.Tests
{
    [SmartConfig]
    public static class EmptyConfig { }

    [SmartConfig]
    [TypeConverter(typeof(JsonToObjectConverter<List<Int32>>))]
    public static class FullConfig
    {
        public static SByte SByte { get; set; }
        public static Byte Byte { get; set; }
        public static Char Char { get; set; }
        public static Int16 Int16 { get; set; }
        public static Int32 Int32 { get; set; }
        public static Int64 Int64 { get; set; }
        public static UInt16 UInt16 { get; set; }
        public static UInt32 UInt32 { get; set; }
        public static UInt64 UInt64 { get; set; }
        public static Single Single { get; set; }
        public static Double Double { get; set; }
        public static Decimal Decimal { get; set; }

        public static String String { get; set; }
        public static bool False { get; set; }
        public static bool True { get; set; }
        public static DateTime DateTime { get; set; }
        public static TestEnum Enum { get; set; }

        public static Color ColorName { get; set; }
        public static Color ColorDec { get; set; }
        public static Color ColorHex { get; set; }

        public static List<int> JsonArray { get; set; }

        [Reusable.Data.Annotations.Optional]
        public static string Optional { get; set; } = "Waldo";

        [Itemized]
        public static int[] ItemizedArray { get; set; }

        [Itemized]
        public static Dictionary<string, int> ItemizedDictionary { get; set; }

        public static class NestedConfig
        {
            public static string NestedString { get; set; }
        }

        [Reusable.Data.Annotations.Ignore]
        public static class IgnoredConfig
        {
            public static string IgnoredString { get; set; } = "Grault";
        }
    }

    [SmartConfig]
    public static class SettingNotFoundConfig
    {
        public static string MissingSetting { get; set; }
    }

    public enum TestEnum
    {
        TestValue1,
        TestValue2,
        TestValue3
    }



    [SmartConfig]
    public class NonStaticConfig { }

    public static class ConfigNotDecorated { }

    [SmartConfig]
    public static class RequiredSettings
    {
        public static int Int32Setting { get; set; }
    }

    [SmartConfig]
    internal static class TestConfig
    {
        [Optional]
        public static string Foo { get; set; } = "Bar";
    }
}