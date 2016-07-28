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
using SmartConfig.Core.Tests.DataStores;
using SmartConfig.DataStores;
// ReSharper disable BuiltInTypeReferenceStyle

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
            var testStore = new TestStore();
            var configuration = Configuration.Load.From(new TestStore()).Select(typeof(EmptyConfig));

            configuration.Type.Verify().IsTrue(x => x == typeof(EmptyConfig));
            configuration.Settings.Count.Verify().IsEqual(0);

            testStore.GetSettingsParameters.Count.Verify().IsEqual(0);
            testStore.SaveSettingsParameters.Count.Verify().IsEqual(0);
        }

        [TestMethod]
        public void LoadByName()
        {
            var store = new TestStore(new MemoryStore
            {
                new Setting { Name = "Foox", Value = "Quux" },
                new Setting { Name = "Foo", Value = "Qux" }
            });
            Configuration.Load.From(store).Select(typeof(SelectorConfig1));

            store.GetSettingsParameters.Count.Verify().IsEqual(1);
            store.GetSettingsParameters[0].IsLike(new Setting(SettingPath.Parse("Foo"), null)).Verify().IsTrue();
            SelectorConfig1.Foo.Verify().IsEqual("Qux");
        }

        [TestMethod]
        public void LoadByNameAndNamespace()
        {
            var store = new TestStore(new MemoryStore
            {
                new Setting { Name = "Foox", Value = "Quux", ["Corge"] = "Waldo" },
                new Setting { Name = "Foo", Value = "Qux", ["Corge"] = "Waldo" },
            });
            Configuration.Load.From(store).Where("Corge", "Waldo").Select(typeof(SelectorConfig2));

            store.GetSettingsParameters.Count.Verify().IsEqual(1);
            store.GetSettingsParameters[0].Verify().IsTrue(x => x.IsLike(new Setting { Name = "Foo", Value = "Qux", ["Corge"] = "Waldo" }));
            SelectorConfig2.Foo.Verify().IsEqual("Qux");
        }

        [TestMethod]
        public void LoadByConfigAndName()
        {
            var store = new TestStore(new MemoryStore
            {
                new Setting { Name = "Bar.Foox", Value = "Quux", ["Corge"] = "Waldo" },
                new Setting { Name = "Bar.Foo", Value = "Qux", ["Corge"] = "Waldo" },
            });
            Configuration.Load.From(store).Where("Corge", "Waldo").Select(typeof(SelectorConfig3));

            store.GetSettingsParameters.Count.Verify().IsEqual(1);
            store.GetSettingsParameters[0].Verify().IsTrue(x => x.IsLike(new Setting { Name = "Bar.Foo", Value = "Qux", ["Corge"] = "Waldo" }));
            SelectorConfig3.Foo.Verify().IsEqual("Qux");
        }

        [TestMethod]
        public void LoadByNameAndNamespaceAndConfig()
        {
            var store = new TestStore(new MemoryStore
            {
                new Setting { Name = "Foox", Value = "Quux", ["Corge"] = "Waldo", Config = "Bar"},
                new Setting { Name = "Foo", Value = "Qux", ["Corge"] = "Waldo", Config = "Bar" },
            });
            Configuration.Load.From(store).Where("Corge", "Waldo").Select(typeof(SelectorConfig4));

            store.GetSettingsParameters.Count.Verify().IsEqual(1);
            store.GetSettingsParameters[0].Verify().IsTrue(x => x.IsLike(new Setting { Name = "Foo", Value = "Qux", ["Corge"] = "Waldo", Config = "Bar" }));
            SelectorConfig4.Foo.Verify().IsEqual("Qux");
        }

        [TestMethod]
        public void LoadAndSkipOptionalSettings()
        {
            Configuration.Load.From(new MemoryStore()).Select(typeof(OptionalConfig));
            OptionalConfig.String.Verify().IsNullOrEmpty();
        }

        [TestMethod]
        public void LoadNestedSettingsWithoutIgnored()
        {
            Configuration.Load.From(new MemoryStore
            {
                { "SubConfig.SubSetting", "foo" },
                { "SubConfig.SubSubConfig1.SubSubSetting", "bar" },
            })
            .Select(typeof(NestedConfig));

            NestedConfig.SubConfig.SubSetting.Verify().IsEqual("foo");
            NestedConfig.SubConfig.SubSubConfig1.SubSubSetting.Verify().IsEqual("bar");
        }

        [TestMethod]
        public void LoadDefaultTypes()
        {
            var culture = CultureInfo.InvariantCulture;

            Configuration.Load.From(new MemoryStore
            {
                { nameof(TypesConfig.SByte), SByte.MaxValue.ToString() },
                { nameof(TypesConfig.Byte), Byte.MaxValue.ToString() },
                { nameof(TypesConfig.Char), Char.MaxValue.ToString() },
                { nameof(TypesConfig.Int16), Int16.MaxValue.ToString() },
                { nameof(TypesConfig.Int32), Int32.MaxValue.ToString() },
                { nameof(TypesConfig.Int64), Int64.MaxValue.ToString() },
                { nameof(TypesConfig.UInt16), UInt16.MaxValue.ToString() },
                { nameof(TypesConfig.UInt32), UInt32.MaxValue.ToString() },
                { nameof(TypesConfig.UInt64), UInt64.MaxValue.ToString() },
                { nameof(TypesConfig.Single), Single.MaxValue.ToString("R", culture) },
                { nameof(TypesConfig.Double), Double.MaxValue.ToString("R", culture) },
                { nameof(TypesConfig.Decimal), Decimal.MaxValue.ToString(culture) },
                //new Setting { Name = nameof(TypesConfig.ColorName), Value = Color.DarkRed.Name },
                //new Setting { Name = nameof(TypesConfig.ColorDec), Value = $"{Color.Plum.R},{Color.Plum.G},{Color.Plum.B}" },
                //new Setting { Name = nameof(TypesConfig.ColorHex), Value = Color.Beige.ToArgb().ToString("X") },
                { nameof(TypesConfig.False), bool.FalseString },
                { nameof(TypesConfig.True), bool.TrueString },
                { nameof(TypesConfig.DateTime), new DateTime().ToString(culture) },
                { nameof(TypesConfig.Enum), TestEnum.TestValue2.ToString() },
                //new Setting { Name = nameof(TypesConfig.ListInt32), Value = "[1, 2, 3]" },
                { nameof(TypesConfig.String), "foo" },
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
       
        // --- itemized tests

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

    }
}

namespace SmartConfig.Core.Tests.Integration.Configuration.Positive.TestConfigs
{
    public enum TestEnum
    {
        TestValue1,
        TestValue2,
        TestValue3
    }

    [SmartConfig]
    public static class EmptyConfig { }

    [SmartConfig]
    public static class SelectorConfig1
    {
        public static string Foo { get; set; }
    }

    [SmartConfig]
    public static class SelectorConfig2
    {
        public static string Foo { get; set; }
    }

    [SmartConfig("Bar")]
    public static class SelectorConfig3
    {
        public static string Foo { get; set; }
    }

    [SmartConfig("Bar", NameOption = ConfigNameOption.AsNamespace)]
    public static class SelectorConfig4
    {
        public static string Foo { get; set; }
    }

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

            public static class SubSubConfig1
            {
                [Optional]
                public static string SubSubSetting { get; set; }
            }

            [SmartUtilities.DataAnnotations.Ignore]
            public static class SubSubConfig2
            {
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
}

namespace SmartConfig.Core.Tests.Integration.Configuration.Negative
{
    using SmartConfig;
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

namespace SmartConfig.Core.Tests.Integration.Configuration.Negative.TestConfigs
{
    [SmartConfig]
    public static class RequiredSettings
    {
        public static int Int32Setting { get; set; }
    }
}
