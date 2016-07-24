using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartUtilities.Collections;
using SmartUtilities.DataAnnotations;
using SmartUtilities.TypeFramework;
using SmartUtilities.TypeFramework.Converters;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;
using SmartConfig.Core.Tests;
using SmartConfig.DataStores;

// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

namespace SmartConfig.Core.Tests.Integration.Configuration.Positive
{
    using SmartConfig;
    using TestConfigs;

    [TestClass]
    public class Load
    {
        [TestMethod]
        public void LoadEmptyConfig()
        {
            var configuration = Configuration.Load.From(new MemoryStore()).Select(typeof(EmptyConfig));

            configuration.Type.Validate().IsTrue(x => x == typeof(EmptyConfig));
            configuration.Settings.Count.Validate().IsEqual(0);
            configuration.SettingReader.Validate().IsNotNull();
            configuration.SettingWriter.Validate().IsNotNull();
        }

        [TestMethod]
        public void LoadDefaultTypes()
        {
            var culture = CultureInfo.InvariantCulture;

            Configuration.Load.From(new MemoryStore
            {
                new Setting { Name = nameof(TypesConfig.SByte), Value = SByte.MaxValue.ToString() },
                new Setting { Name = nameof(TypesConfig.Byte), Value = Byte.MaxValue.ToString() },
                new Setting { Name = nameof(TypesConfig.Char), Value = Char.MaxValue.ToString() },
                new Setting { Name = nameof(TypesConfig.Int16), Value = Int16.MaxValue.ToString() },
                new Setting { Name = nameof(TypesConfig.Int32), Value = Int32.MaxValue.ToString() },
                new Setting { Name = nameof(TypesConfig.Int64), Value = Int64.MaxValue.ToString() },
                new Setting { Name = nameof(TypesConfig.UInt16), Value = UInt16.MaxValue.ToString() },
                new Setting { Name = nameof(TypesConfig.UInt32), Value = UInt32.MaxValue.ToString() },
                new Setting { Name = nameof(TypesConfig.UInt64), Value = UInt64.MaxValue.ToString() },
                new Setting { Name = nameof(TypesConfig.Single), Value = Single.MaxValue.ToString("R", culture) },
                new Setting { Name = nameof(TypesConfig.Double), Value = Double.MaxValue.ToString("R", culture) },
                new Setting { Name = nameof(TypesConfig.Decimal), Value = Decimal.MaxValue.ToString(culture) },
                //new Setting { Name = nameof(TypesConfig.ColorName), Value = Color.DarkRed.Name },
                //new Setting { Name = nameof(TypesConfig.ColorDec), Value = $"{Color.Plum.R},{Color.Plum.G},{Color.Plum.B}" },
                //new Setting { Name = nameof(TypesConfig.ColorHex), Value = Color.Beige.ToArgb().ToString("X") },
                new Setting { Name = nameof(TypesConfig.False), Value = bool.FalseString },
                new Setting { Name = nameof(TypesConfig.True), Value = bool.TrueString },
                new Setting { Name = nameof(TypesConfig.DateTime), Value = new DateTime().ToString(culture) },
                new Setting { Name = nameof(TypesConfig.Enum), Value = TestEnum.TestValue2.ToString() },
                //new Setting { Name = nameof(TypesConfig.ListInt32), Value = "[1, 2, 3]" },
                new Setting { Name = nameof(TypesConfig.String), Value = "foo" },
                //new Setting { Name = nameof(TypesConfig.Uri), Value = bool.TrueString },
                //new Setting { Name = nameof(TypesConfig.XDocument), Value = @"<?xml version=""1.0""?><testXml></testXml>" },
                //new Setting { Name = nameof(TypesConfig.XElement), Value = @"<testXml></testXml>" },
            })
            .Select(typeof(TypesConfig));

            TypesConfig.SByte.Validate().IsEqual(SByte.MaxValue);
            TypesConfig.Enum.Validate().IsEqual(TestEnum.TestValue2);
            //TypesConfig.ColorName.ToArgb().Validate().IsEqual(Color.Red.ToArgb());
            //TypesConfig.ColorDec.ToArgb().Validate().IsEqual(Color.Plum.ToArgb());
            //TypesConfig.ColorHex.ToArgb().Validate().IsEqual(Color.Beige.ToArgb());
            //TypesConfig.XDocument.ToString().Validate().IsEqual(XDocument.Parse(@"<?xml version=""1.0""?><testXml></testXml>").ToString());
            //TypesConfig.XElement.ToString().Validate().IsEqual(XElement.Parse(@"<testXml></testXml>").ToString());
            //TypesConfig.ListInt32.SequenceEqual(new[] { 1, 2, 3 }).Validate().IsTrue();
        }

        [TestMethod]
        public void LoadItemizedArray()
        {
            Configuration.Load.From(new MemoryStore
            {
                { "Numbers", "3" },
                { "Numbers", "7" },
                { "Foo", "Bar" }
            })
            .Select(typeof(ItemizedArrayConfig));

            ItemizedArrayConfig.Numbers.Length.Validate().IsEqual(2);
        }

        [TestMethod]
        public void LoadItemizedList()
        {
            Configuration.Load.From(new MemoryStore
            {
                { "Numbers", "3" },
                { "Numbers", "7" },
                { "Foo", "Bar" }
            })
            .Select(typeof(ItemizedListConfig));

            ItemizedListConfig.Numbers.Count.Validate().IsEqual(2);
        }

        [TestMethod]
        public void LoadItemizedHashSet()
        {
            Configuration.Load.From(new MemoryStore
            {
                { "Numbers", "3" },
                { "Numbers", "7" },
                { "Foo", "Bar" }
            })
            .Select(typeof(ItemizedHashSetConfig));

            ItemizedHashSetConfig.Numbers.Count.Validate().IsEqual(2);
        }

        [TestMethod]
        public void LoadItemizedDictionary()
        {
            Configuration.Load.From(new MemoryStore
            {
                { "Numbers[Foo]", "3" },
                { "Numbers[Bar]", "7" },
                { "Foo", "Bar" }
            })
            .Select(typeof(ItemizedDictionaryConfig));

            ItemizedDictionaryConfig.Numbers.Count.Validate().IsEqual(2);
        }

        [TestMethod]
        public void LoadWithNamespaces()
        {
            Configuration.Load.From(new MemoryStore
            {
                new Setting
                {
                    [nameof(Setting.Name)] = (SettingPath)"foo",
                    [nameof(Setting.Value)] = "bar",
                    ["testnamespace"] = "qux",
                },
                new Setting
                {
                    [nameof(Setting.Name)] = (SettingPath)"foo",
                    [nameof(Setting.Value)] = "bax",
                    ["testnamespace"] = "quo",
                }
            })
            .Where("testnamespace", "quo")
            .Select(typeof(NamespaceConfig));

            NamespaceConfig.Foo.Verify().IsEqual("bax");
        }

        [TestMethod]
        public void IgnoreOptionalSettings()
        {
            new Action(() =>
                Configuration.Load.From(new MemoryStore()).Select(typeof(OptionalConfig))
            ).Validate().DoesNotThrow();
        }

        [TestMethod]
        public void LoadNestedSettings()
        {
            Configuration.Load.From(new MemoryStore
            {
                { "SubConfig.SubSetting", "foo" },
                { "SubConfig.SubSubConfig.SubSubSetting", "bar" },
            })
            .Select(typeof(NestedConfig));

            NestedConfig.SubConfig.SubSetting.Verify().IsEqual("foo");
            NestedConfig.SubConfig.SubSubConfig.SubSubSetting.Verify().IsEqual("bar");
        }

    }
}

namespace SmartConfig.Core.Tests.Integration.Configuration.Positive.TestConfigs
{
    [SmartConfig]
    public static class EmptyConfig { }

    [SmartConfig]
    public static class TypesConfig
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
        //public static Color ColorName { get; set; }
        //public static Color ColorDec { get; set; }
        //public static Color ColorHex { get; set; }
        public static bool False { get; set; }
        public static bool True { get; set; }
        public static DateTime DateTime { get; set; }
        public static TestEnum Enum { get; set; }
        //public static List<int> ListInt32 { get; set; }
        public static String String { get; set; }
        //public static Uri Uri { get; set; }
        //public static XDocument XDocument { get; set; }
        //public static XElement XElement { get; set; }
    }

    [SmartConfig]
    public static class OptionalConfig
    {
        [Optional]
        public static string String { get; set; }
    }

    [SmartConfig]
    public static class NestedConfig
    {
        public static class SubConfig
        {
            [Optional]
            public static string SubSetting { get; set; }

            public static class SubSubConfig
            {
                [Optional]
                public static string SubSubSetting { get; set; }
            }
        }
    }

    [SmartConfig]
    public static class ItemizedDictionaryConfig
    {
        [Itemized]
        public static Dictionary<string, int> Numbers { get; set; }
    }

    [SmartConfig]
    public static class ItemizedListConfig
    {
        [Itemized]
        public static List<int> Numbers { get; set; }
    }

    [SmartConfig]
    public static class ItemizedHashSetConfig
    {
        [Itemized]
        public static HashSet<int> Numbers { get; set; }
    }

    [SmartConfig]
    public static class ItemizedArrayConfig
    {
        [Itemized]
        public static int[] Numbers { get; set; }
    }

    [SmartConfig]
    public static class NamespaceConfig
    {
        public static string Foo { get; set; }
    }
}

namespace SmartConfig.Core.Tests.IntegrationTests.ErrorHandling
{
    using TestConfigs;

    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void RequiredSettingNotFound()
        {
            new Action(() =>
                Configuration.Load.From(new MemoryStore()).Select(typeof(RequiredSettings))
            ).Validate().Throws<AggregateException>();
        }

        [TestMethod]
        public void PropertyNameNullOrEmpty()
        {
            new Action(() => Configuration.Load.From(new MemoryStore()).Where(null, null)).Validate().Throws<ValidationException>();
            new Action(() => Configuration.Load.From(new MemoryStore()).Where(string.Empty, null)).Validate().Throws<ValidationException>();
        }

        [TestMethod]
        public void ValueNull()
        {
            new Action(() => Configuration.Load.From(new MemoryStore()).Where("foo", null)).Validate().Throws<ValidationException>();
        }
    }


    [TestClass]
    public class Load_UseCases
    {



    }
}

namespace SmartConfig.Core.Tests.IntegrationTests.ErrorHandling.TestConfigs
{
    [SmartConfig]
    public static class RequiredSettings
    {
        public static int Int32Setting { get; set; }
    }
}
