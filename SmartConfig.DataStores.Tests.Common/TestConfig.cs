using System;
using System.Collections.Generic;
using System.Drawing;
using Reusable;
using Reusable.Data.Annotations;
using SmartConfig.Data.Annotations;
using Reusable.TypeConversion;
using Reusable.StringFormatting.Formatters;
// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle

namespace SmartConfig.DataStores.Tests.Common
{
    [SmartConfig]
    [TypeConverter(typeof(JsonToObjectConverter<List<Int32>>))]
    [TypeConverter(typeof(ObjectToJsonConverter<List<Int32>>))]
    [SettingNameUnset]
    public static class TestConfig
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
        [Format("R")]
        public static Single Single { get; set; }
        [Format("R")]
        public static Double Double { get; set; }
        public static Decimal Decimal { get; set; }

        public static String StringDE { get; set; }
        public static String StringPL { get; set; }
        public static Boolean Boolean { get; set; }
        public static TestEnum Enum { get; set; }

        public static DateTime DateTime { get; set; }

        [Format("#RRGGBB", typeof(HexadecimalColorFormatter))]
        public static Color ColorName { get; set; }

        [Format("#RRGGBB", typeof(HexadecimalColorFormatter))]
        public static Color ColorDec { get; set; }

        [Format("#RRGGBB", typeof(HexadecimalColorFormatter))]
        public static Color ColorHex { get; set; }

        public static List<int> JsonArray { get; set; }        

        [Itemized]
        public static int[] ArrayInt32 { get; set; }

        [Itemized]
        public static Dictionary<string, int> DictionaryStringInt32 { get; set; }

        [Optional]
        public static string OptionalString { get; set; } = "Optional value";

        public static class NestedConfig
        {
            public static string NestedString { get; set; }
        }

        [Ignore]
        public static class IgnoredConfig
        {
            public static string IgnoredString { get; set; } = "Ignored value";
        }
    }

    public enum TestEnum
    {
        TestValue1,
        TestValue2,
        TestValue3
    }
}