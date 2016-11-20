using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable.Data.DataAnnotations;
using Reusable.Testing;
using Reusable.Testing.Validations;
using Reusable.Validations;
using SmartConfig.DataAnnotations;
using SmartConfig.DataStores.SqlServer;

namespace SmartConfig.DataStores.SqlServer.Tests.Integration
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable once CheckNamespace

    using SqlServerTestConfigs;

    [TestClass]
    public class SqlServerTest
    {
        [TestMethod]
        public void GetSettings_SimpleSetting()
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
        public void GetSettings_WithConfigAsPath()
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
        public void GetSettings_WithConfigAsNamespace()
        {
            Configuration.Load
                .FromSqlServer("name=SmartConfigTest", builder => builder.TableName("Setting2").Column("Corge"))
                .Where("Corge", "waldo")
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

        [TestMethod]
        public void GetSettings_Throws_ColumnConfigurationNotFoundException()
        {
            new Action(() =>
            {
                Configuration.Load
                    .FromSqlServer("name=SmartConfigTest", builder => builder.TableName("Setting2"))
                    .Where("Corge", "waldo")
                    .Select(typeof(FullConfig3));
            })
            .Verify()
            .Throws<ConfigurationLoadException>(ex =>
            {
                ex.InnerException.Verify().IsInstanceOfType(typeof(ColumnConfigurationNotFoundException));
            });
        }

        [TestMethod]
        public void SaveSetting_ByName()
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
        public void SaveSettings_ByNameAndNamespaceFromExpression()
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
            Configuration.Reload(typeof(TestConfig2));
            TestConfig2.Bar.Verify().IsNotNullOrEmpty().IsEqual(newValue);
        }

        [TestMethod]
        public void SaveSettings_Itemized()
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
            Configuration.Reload(typeof(ItemizedConfig));

            ItemizedConfig.Numbers2.Count.Verify().IsEqual(2);
        }
    }
}

namespace SmartConfig.DataStores.SqlServer.Tests.Integration.SqlServerTestConfigs
{
    internal static class BaseConfig
    {
        public static string Environment => "Itemized";
    }

    [SmartConfig]
    internal static class TestConfig1
    {
        public static string Roxy { get; set; }
    }

    [SmartConfig]
    internal static class TestConfig2
    {
        [Optional]
        public static string Bar { get; set; }
    }

    [SmartConfig]
    internal static class ItemizedConfig
    {
        [Itemized]
        [Optional]
        public static Dictionary<string, int> Numbers2 { get; set; }
    }
}