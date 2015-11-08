using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.Tests.TestConfigs;
// ReSharper disable once InconsistentNaming

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class DbSourceTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            var connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;
            using (var context = SmartConfigContext<TestSetting>.Create(connectionString, "TestConfig", KeyNames.From<TestSetting>()))
            {
                context.Database.Initialize(true);
            }
        }

        [TestMethod]
        public void Select_Int32Field()
        {
            var dataSource = new DbSource<TestSetting>(
                ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                "TestConfig",
                new[]
                {
                    new CustomKey(KeyNames.EnvironmentKeyName, "ABC", Filters.FilterByString),
                    new CustomKey(KeyNames.VersionKeyName, "1.3.0", Filters.FilterByVersion)
                });

            Assert.AreEqual("123", dataSource.Select("Int32Setting"));
        }

        [TestMethod]
        public void Update_Int32Fields()
        {
            var dataSource = new DbSource<TestSetting>(
                ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                "TestConfig",
                new[]
                {
                    new CustomKey(KeyNames.EnvironmentKeyName, "ABC", Filters.FilterByString),
                    new CustomKey(KeyNames.VersionKeyName, "1.3.0", Filters.FilterByVersion)
                });

            Assert.AreEqual("123", dataSource.Select("Int32Setting"));

            dataSource.Update("Int32Setting", "456");
            Assert.AreEqual("456", dataSource.Select("Int32Setting"));

            dataSource.Update("Int32Setting", "789");
            Assert.AreEqual("789", dataSource.Select("Int32Setting"));
        }
    }

    [TestClass]
    public class DbSource_Select_Method
    {
        private const string TestTableName = "TestSetting";

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            var connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;
            using (var context = SmartConfigContext<TestSetting>.Create(connectionString, TestTableName, KeyNames.From<TestSetting>()))
            {
                context.Database.Initialize(true);
            }
        }

        [TestMethod]
        public void CanSelectSettingsByName()
        {
            var dataSource = new DbSource<TestSetting>(
                ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                TestTableName,
                new[]
                {
                    new CustomKey(KeyNames.EnvironmentKeyName, "ABC", Filters.FilterByString),
                    new CustomKey(KeyNames.VersionKeyName, "1.3.0", Filters.FilterByVersion)
                });

            Assert.AreEqual("123", dataSource.Select("Int32Setting"));
        }
    }
}
