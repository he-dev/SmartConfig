using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Data.DataAnnotations;
using Reusable.Testing;
using Reusable.Validations;
using SmartConfig.DataAnnotations;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace SmartConfig.DataStores.SqlServer.Tests.Integration.SqlServerStore.Positive
{
    using SqlServer;

    [TestClass]
    public class FullTests
    {
        [TestMethod]
        public void SimpleSetting()
        {
            Configuration.Load
                .FromSqlServer("name=SmartConfigTest", configure => configure.TableName("Setting1"))
                .Select(typeof(FullConfig1));

            FullConfig1.StringSetting.Verify().IsNotNullOrEmpty().IsEqual("Foo");
            FullConfig1.ArraySetting.Length.Verify().IsBetweenOrEqual(2, 3);
            FullConfig1.ArraySetting.Contains(5).Verify().IsTrue();
            FullConfig1.ArraySetting.Contains(8).Verify().IsTrue();
            if (FullConfig1.ArraySetting.Length == 3)
            {
                FullConfig1.ArraySetting.Contains(13).Verify().IsTrue();
            }
            FullConfig1.DictionarySetting.Count.Verify().IsBetweenOrEqual(2, 3);
            FullConfig1.DictionarySetting["foo"].Verify().IsEqual(21);
            FullConfig1.DictionarySetting["bar"].Verify().IsEqual(34);
            if (FullConfig1.DictionarySetting.Count == 3)
            {
                FullConfig1.DictionarySetting["baz"].Verify().IsEqual(55);
            }
            FullConfig1.NestedConfig.StringSetting.Verify().IsEqual("Bar");
            FullConfig1.IgnoredConfig.StringSetting.Verify().IsEqual("Grault");

            if (FullConfig1.ArraySetting.Length == 2) FullConfig1.ArraySetting = new[] { 5, 8, 13 };
            else if (FullConfig1.ArraySetting.Length == 3) FullConfig1.ArraySetting = new[] { 5, 8 };

            if (FullConfig1.DictionarySetting.Count == 2) FullConfig1.DictionarySetting["baz"] = 55;
            else if (FullConfig1.DictionarySetting.Count == 3) FullConfig1.DictionarySetting.Remove("baz");

            Configuration.Save(typeof(FullConfig1));
        }

        [TestMethod]
        public void GetSettingsWithConfigAsPath()
        {
            Configuration.Load
                .FromSqlServer("name=SmartConfigTest", configure => configure.TableName("Setting1"))
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
                .FromSqlServer("name=SmartConfigTest", configure => configure.TableName("Setting2").Column("Corge", SqlDbType.NVarChar, 200))
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
            Configuration.TryReload(typeof(TestConfig1));
            TestConfig1.Roxy.Verify().IsNotNullOrEmpty().IsEqual(newValue);
        }

        [TestMethod]
        public void SaveSettingByNameAndNamespaceFromExpression()
        {
            Configuration.Load
                .FromSqlServer("name=SmartConfigTest", configure => configure.Column(() => BaseConfig.Environment))
                .Where("Environment", "Qux")
                .Select(typeof(TestConfig2));

            var rnd = new Random();
            var newValue = $"{nameof(TestConfig2.Bar)}{rnd.Next(101)}";
            TestConfig2.Bar = newValue;

            Configuration.Save(typeof(TestConfig2));
            TestConfig2.Bar = string.Empty;
            Configuration.TryReload(typeof(TestConfig2));
            TestConfig2.Bar.Verify().IsNotNullOrEmpty().IsEqual(newValue);
        }

        [TestMethod]
        public void SaveItemizedSettings()
        {
            Configuration.Load
                .FromSqlServer("name=SmartConfigTest", configure => configure.Column("Environment", SqlDbType.NVarChar, 300))
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
            Configuration.TryReload(typeof(ItemizedConfig));

            ItemizedConfig.Numbers2.Count.Verify().IsEqual(2);
        }

        private static class BaseConfig
        {
            public static string Environment => "Itemized";
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