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
        public void GetSettingsByName()
        {
            var config = Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest"))
                .Select(typeof(TestConfig));

            config.Settings.Count.Validate().IsEqual(1);
            TestConfig.Foo.Verify("TestConfig.Foo").IsEqual("Baar");
        }

        [TestMethod]
        public void GetSettingsByNameAndNamespace()
        {
            var config = Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest"))
                .Where("Environment", "Corge")
                .Select(typeof(TestConfig2));

            config.Settings.Count.Validate().IsEqual(1);
            TestConfig2.Rox.Verify("TestConfig.Rox").IsNotNullOrEmpty().IsEqual("Quux");
        }

        [TestMethod]
        public void GetItemizedDictionary()
        {
            var config = Configuration.Load
                .From(new SqlServerStore("name=SmartConfigTest"))
                .Where("Environment", "Itemized")
                .Select(typeof(ItemizedDictionary));

            config.Settings.Count.Validate().IsEqual(1);
            ItemizedDictionary.Numbers.Count.Verify("ItemizedDictionary.Numbers").IsEqual(2);
        }

        [SmartConfig]
        private static class TestConfig
        {
            public static string Foo { get; set; }
        }

        [SmartConfig]
        private static class TestConfig2
        {
            public static string Rox { get; set; }
        }

        [SmartConfig]
        public static class ItemizedDictionary
        {
            [Itemized]
            public static Dictionary<string, int> Numbers { get; set; }
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