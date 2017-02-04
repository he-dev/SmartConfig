using System;
using System.Collections.Generic;
using System.Drawing;
using Reusable.Converters;
using Reusable.Data.Annotations;
using Reusable.Formatters;
using SmartConfig.Data.Annotations;
// ReSharper disable BuiltInTypeReferenceStyle

namespace SmartConfig.Core.Tests
{
    [SmartConfig]
    public static class EmptyConfig { }

    [SmartConfig]
    public static class IntegralConfig
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
        public static TestEnum Enum { get; set; }
    }

    [SmartConfig]
    public static class DateTimeConfig
    {
        public static DateTime DateTime { get; set; }
    }

    [SmartConfig]
    public static class ColorConfig
    {
        [Format(typeof(HexadecimalColorFormatter), "#RRGGBB")]
        public static Color ColorName { get; set; }

        [Format(typeof(HexadecimalColorFormatter), "#RRGGBB")]
        public static Color ColorDec { get; set; }

        [Format(typeof(HexadecimalColorFormatter), "#RRGGBB")]
        public static Color ColorHex { get; set; }
    }

    [SmartConfig]
    [TypeConverter(typeof(JsonToObjectConverter<List<Int32>>))]
    [TypeConverter(typeof(ObjectToJsonConverter<List<Int32>>))]
    public static class JsonConfig
    {
        public static List<int> JsonArray { get; set; }
    }

    [SmartConfig]
    public static class ItemizedArrayConfig
    {
        [Itemized]
        public static int[] ItemizedArray { get; set; }
    }

    [SmartConfig]
    public static class ItemizedDictionaryConfig
    {
        [Itemized]
        public static Dictionary<string, int> ItemizedDictionary { get; set; }
    }

    [SmartConfig]
    public static class NestedConfig
    {
        public static class SubConfig
        {
            public static string NestedString { get; set; }
        }
    }

    [SmartConfig]
    public static class IgnoredConfig
    {
        [Ignore]
        public static class SubConfig
        {
            public static string IgnoredString { get; set; } = "Grault";
        }
    }

    [SmartConfig]
    public static class OptionalConfig
    {
        [Optional]
        public static string OptionalSetting { get; set; } = "Waldo";
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