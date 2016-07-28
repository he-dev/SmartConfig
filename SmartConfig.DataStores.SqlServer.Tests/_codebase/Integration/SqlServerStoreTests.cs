using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartUtilities.DataAnnotations;
using SmartUtilities.ValidationExtensions;
using SmartUtilities.ValidationExtensions.Testing;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace SmartConfig.DataStores.SqlServer.Tests.Integration.SqlServerStore.Positive
{
    using SqlServer;

    [TestClass]
    public class GetSettings
    {
        [TestMethod]
        public void GetSettingsSimple()
        {
            Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest", builder => builder.TableName("Setting1")))
                .Select(typeof(FullConfig1));

            FullConfig1.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foo");
            FullConfig1.ArraySetting.Length.Verify().IsEqual(2);
            FullConfig1.DictionarySetting.Count.Verify().IsEqual(2);
            FullConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
            FullConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }

        [TestMethod]
        public void GetSettingsWithConfigAsPath()
        {
            Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest", builder => builder.TableName("Setting1")))
                .Select(typeof(FullConfig2));

            FullConfig2.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foox");
            FullConfig2.ArraySetting.Length.Verify().IsEqual(2);
            FullConfig2.DictionarySetting.Count.Verify().IsEqual(2);
            FullConfig2.NestedConfig.StringSetting.Verify().IsEqual("Barx");
            FullConfig2.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }

        [TestMethod]
        public void GetSettingsWithConfigAsNamespace()
        {
            Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest", builder => builder.TableName("Setting2")))
                .Where("corge", "waldo")
                .Select(typeof(FullConfig3));

            FullConfig3.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Fooxy");
            FullConfig3.ArraySetting.Length.Verify().IsEqual(2);
            FullConfig3.ArraySetting[0].Verify().IsEqual(31);
            FullConfig3.ArraySetting[1].Verify().IsEqual(71);
            FullConfig3.DictionarySetting.Count.Verify().IsEqual(2);
            FullConfig3.DictionarySetting["foo"].Verify().IsEqual(42);
            FullConfig3.DictionarySetting["bar"].Verify().IsEqual(82);
            FullConfig3.NestedConfig.StringSetting.Verify().IsEqual("Barxy");
            FullConfig3.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");
        }
    }

    [TestClass]
    public class SaveSetting
    {
        [TestMethod]
        public void SaveSettingByName()
        {
            Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest"))
                .Select(typeof(TestConfig1));

            var rnd = new Random();
            var newValue = $"{nameof(TestConfig1.Roxy)}{rnd.Next(101)}";
            TestConfig1.Roxy = newValue;

            Configuration.Save(typeof(TestConfig1));
            TestConfig1.Roxy = string.Empty;
            Configuration.Reload(typeof(TestConfig1));
            TestConfig1.Roxy.Verify().IsNotNullOrEmpty().IsEqual(newValue);
        }

        [TestMethod]
        public void SaveSettingByNameAndNamespace()
        {
            Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest"))
                .Where("Environment", "Qux")
                .Select(typeof(TestConfig2));

            var rnd = new Random();
            var newValue = $"{nameof(TestConfig2.Bar)}{rnd.Next(101)}";
            TestConfig2.Bar = newValue;

            Configuration.Save(typeof(TestConfig2));
            TestConfig2.Bar = string.Empty;
            Configuration.Reload(typeof(TestConfig2));
            TestConfig2.Bar.Verify().IsNotNullOrEmpty().IsEqual(newValue);
        }

        [TestMethod]
        public void SaveItemizedSettings()
        {
            Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest"))
                .Where("Environment", "Itemized")
                .Select(typeof(ItemizedConfig));

            ItemizedConfig.Numbers2 = new Dictionary<string, int>();

            // add few items
            for (var i = 0; i < 3; i++)
            {
                ItemizedConfig.Numbers2.Add(((char)(97 + i)).ToString(), 65 + i);
            }

            Configuration.Save(typeof(ItemizedConfig));

            ItemizedConfig.Numbers2.Remove(ItemizedConfig.Numbers2.ElementAt(1).Key);
            Configuration.Save(typeof(ItemizedConfig));
            Configuration.Reload(typeof(ItemizedConfig));

            ItemizedConfig.Numbers2.Count.Verify().IsEqual(2);
        }

        [SmartConfig]
        private static class TestConfig1
        {
            public static string Roxy { get; set; }
        }

        [SmartConfig]
        private static class TestConfig2
        {
            [Optional]
            public static string Bar { get; set; }
        }

        [SmartConfig]
        private static class ItemizedConfig
        {
            [Itemized]
            [Optional]
            public static Dictionary<string, int> Numbers2 { get; set; }
        }
    }
}

namespace SmartConfig.DataStores.SqlServer.Tests.Integration.SqlServerStore.Negative
{
}