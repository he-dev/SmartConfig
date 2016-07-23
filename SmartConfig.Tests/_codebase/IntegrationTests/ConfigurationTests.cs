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

// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

namespace SmartConfig.Core.Tests.Integration.ConfigurationTests.Positive
{
    using SmartConfig;
    using TestConfigs;

    [TestClass]
    public class LoadTests
    {
        [TestMethod]
        public void LoadEmptyConfig()
        {
            var configuration = Configuration.Load.From(new NullStore()).Select(typeof(EmptyConfig));

            configuration.Type.Validate().IsTrue(x => x == typeof(EmptyConfig));
            configuration.Settings.Count.Validate().IsEqual(0);
            configuration.SettingReader.Validate().IsNotNull();
            configuration.SettingWriter.Validate().IsNotNull();
        }

        [TestMethod]
        public void LoadDefaultTypes()
        {
            var culture = CultureInfo.InvariantCulture;

            var testData = new AutoKeyDictionary<string, Setting>(x => x.Name)
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
            };

            Configuration.Load
                .From(new TestStore
                {
                    GetSettingsFunc = (name, props) => testData[name.ToString()].AsEnumerable().ToList()
                })
                .Register<StringToEnumConverter<TestEnum>>()
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
        public void LoadItemizedDictionary()
        {
            var testData = new AutoKeyDictionary<string, Setting>(x => x.Name)
            {
                new Setting { Name = "Numbers[Foo]", Value = "3" },
                new Setting { Name = "Numbers[Bar]", Value = "7" },
            };

            Configuration.Load.From(new TestStore
            {
                GetSettingsFunc = (path, props) => testData.Values.Where(x => x.Name.ToString().StartsWith(path)).ToList()
            })
            .Select(typeof(ItemizedDictionaryConfig));

            ItemizedDictionaryConfig.Numbers.Count.Validate().IsEqual(2);
        }

        [TestMethod]
        public void LoadItemizedList()
        {
            var testData = new []
            {
                new Setting { Name = "Numbers", Value = "3" },
                new Setting { Name = "Numbers", Value = "7" },
            };

            Configuration.Load.From(new TestStore
            {
                GetSettingsFunc = (path, props) => testData.Where(x => x.Name.ToString().StartsWith(path)).ToList()
            })
            .Select(typeof(ItemizedListConfig));

            ItemizedListConfig.Numbers.Count.Validate().IsEqual(2);
        }

        [TestMethod]
        public void LoadItemizedHashSet()
        {
            var testData = new[]
            {
                new Setting { Name = "Numbers", Value = "3" },
                new Setting { Name = "Numbers", Value = "7" },
            };

            Configuration.Load.From(new TestStore
            {
                GetSettingsFunc = (path, props) => testData.Where(x => x.Name.ToString().StartsWith(path)).ToList()
            })
            .Select(typeof(ItemizedHashSetConfig));

            ItemizedHashSetConfig.Numbers.Count.Validate().IsEqual(2);
        }

        [TestMethod]
        public void LoadItemizedArray()
        {
            var testData = new[]
            {
                new Setting { Name = "Numbers", Value = "3" },
                new Setting { Name = "Numbers", Value = "7" },
            };

            Configuration.Load.From(new TestStore
            {
                GetSettingsFunc = (path, props) => testData.Where(x => x.Name.ToString().StartsWith(path)).ToList()
            })
            .Select(typeof(ItemizedArrayConfig));

            ItemizedArrayConfig.Numbers.Length.Validate().IsEqual(2);
        }

        [TestMethod]
        public void CreateConfigurationWithNamespaces()
        {
            var configuration = Configuration.Load.From(new NullStore()).Where("Foo", "Bar").Select(typeof(EmptyConfig));
            configuration.SettingReader.Namespaces.Count.Validate().IsEqual(1);
            configuration.SettingReader.Namespaces.Validate().IsTrue(x => x.ContainsKey("foo"));
            configuration.SettingReader.Namespaces["foo"].Validate().IsTrue(x => (string)x == "Bar");
        }

        [TestMethod]
        public void IgnoreOptionalSettings()
        {
            new Action(() =>
                Configuration.Load.From(new NullStore()).Select(typeof(OptionalSettings))
            ).Validate().DoesNotThrow();
        }

        [TestMethod]
        public void ReadNestedSettings()
        {
            var configuration = Configuration.Load.From(new NullStore()).Select(typeof(NestedSettings));

            configuration.Settings.Count.Validate().IsEqual(2);
            configuration.Settings.SingleOrDefault(x => x.SettingPath.ToString() == "SubConfig.SubSetting").Validate().IsNotNull();
            configuration.Settings.SingleOrDefault(x => x.SettingPath.ToString() == "SubConfig.SubSubConfig.SubSubSetting").Validate().IsNotNull();
        }

    }
}

namespace SmartConfig.Core.Tests.Integration.ConfigurationTests.Positive.TestConfigs
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
    public static class OptionalSettings
    {
        [Optional]
        public static string String { get; set; }
    }

    [SmartConfig]
    public static class NestedSettings
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
                Configuration.Load.From(new NullStore()).Select(typeof(RequiredSettings))
            ).Validate().Throws<AggregateException>();
        }

        [TestMethod]
        public void PropertyNameNullOrEmpty()
        {
            new Action(() => Configuration.Load.From(new NullStore()).Where(null, null)).Validate().Throws<ValidationException>();
            new Action(() => Configuration.Load.From(new NullStore()).Where(string.Empty, null)).Validate().Throws<ValidationException>();
        }

        [TestMethod]
        public void ValueNull()
        {
            new Action(() => Configuration.Load.From(new NullStore()).Where("foo", null)).Validate().Throws<ValidationException>();
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
