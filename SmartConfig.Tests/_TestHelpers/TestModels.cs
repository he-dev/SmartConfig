using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SmartConfig.DataAnnotations;
using SmartUtilities.ObjectConverters.DataAnnotations;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Core.Tests.TestModels
{
    namespace Features
    {
        [SmartConfig]
        public static class BooleanSettings
        {
            public static bool falseSetting { get; set; }
            public static bool trueSetting { get; set; }
        }

        [SmartConfig]
        public static class ColorSettings
        {
            public static Color NameColorSetting { get; set; }
            public static Color DecColorSetting { get; set; }
            public static Color HexColorSetting { get; set; }
        }       

        [SmartConfig]
        public static class DateTimeSettings
        {
            public static DateTime DateTimeSetting { get; set; }
        }

        [SmartConfig]
        public static class EnumSettings
        {
            public static TestEnum EnumSetting { get; set; }
        }

        [SmartConfig]
        public static class JsonSettings
        {
            public static List<int> ListInt32Setting { get; set; }
        }

        [SmartConfig]
        public static class NestedSettings
        {
            public static class SubConfig
            {
                public static string SubSetting { get; set; }

                public static class SubSubConfig
                {
                    public static string SubSubSetting { get; set; }
                }
            }
        }

        [SmartConfig]
        public static class NonOptionalSettings
        {
            public static int Int32Setting { get; set; }
        }

        [SmartConfig]
        public static class NumericSettings
        {
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

        [SmartConfig]
        public static class OptionalSettings
        {
            [Optional]
            public static string StringSetting { get; set; } = "abc";
        }

        [SmartConfig]
        public static class StringSettings
        {
            public static string StringSetting { get; set; }
        }

        [SmartConfig]
        public static class UnsupportedTypeSettings
        {
            public static Uri UriSetting { get; set; }
        }      
       
        [SmartConfig]
        public static class XmlSettings
        {
            public static XDocument XDocumentSetting { get; set; }
            public static XElement XElementSetting { get; set; }
        }

    }

    namespace Constraints
    {
        [SmartConfig]
        public static class CustomDateTimeFormatSettings
        {
            [DateTimeFormat("ddMMMyy")]
            public static DateTime DateTimeSetting { get; set; }
        }

        [SmartConfig]
        public static class CustomRangeSettings
        {
            [Range(typeof(int), "3", "7")]
            public static int Int32Field { get; set; }
        }

        [SmartConfig]
        public static class CustomRegularExpressionSettings
        {
            [RegularExpression(@"\d{2}")]
            public static string StringSetting { get; set; }
        }
    }
}
