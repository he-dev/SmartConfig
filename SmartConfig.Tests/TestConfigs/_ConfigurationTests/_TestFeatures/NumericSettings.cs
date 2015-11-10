// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using System.Globalization;
using SmartConfig.Data;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class NumericSettings
    {
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        public static sbyte sbyteSetting { get; set; }
        public static byte byteSetting { get; set; }
        public static char charSetting { get; set; }
        public static short shortSetting { get; set; }
        public static ushort ushortSetting { get; set; }
        public static int intSetting { get; set; }
        public static uint uintSetting { get; set; }
        public static long longSetting { get; set; }
        public static ulong ulongSetting { get; set; }
        public static float floatSetting { get; set; }
        public static double doubleSetting { get; set; }
        public static decimal decimalSetting { get; set; }
    }
}
