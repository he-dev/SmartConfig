using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Tests.TestConfigs;
using SmartUtilities.UnitTesting;
// ReSharper disable InconsistentNaming


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
            using (var context = new SmartConfigContext<TestSetting>(connectionString, "TestConfig"))
            {
                context.Database.Initialize(true);
            }
        }

        [TestMethod]
        public void Select_Int32Field()
        {
            var dataSource = new DbSource<TestSetting>(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString, "TestConfig");

            var keys = new[]
            {
                new SettingKey(Setting.DefaultKeyName, "Int32Setting"),
                new SettingKey("Environment", "ABC"),
                new SettingKey("Version", "1.3.0"),
            };
            Assert.AreEqual("123", dataSource.Select(keys));
        }

        [TestMethod]
        public void Update_Int32Fields()
        {
            //var dataSource = new DbSource<TestSetting>(
            //    ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
            //    "TestConfig",
            //    new[]
            //    {
            //        new SettingKey(SettingKeyNameReadOnlyCollection.EnvironmentKeyName, "ABC", Filters.FilterByString),
            //        new SettingKey(SettingKeyNameReadOnlyCollection.VersionKeyName, "1.3.0", Filters.FilterByVersion)
            //    });

            //Assert.AreEqual("123", dataSource.Select("Int32Setting"));

            //dataSource.Update("Int32Setting", "456");
            //Assert.AreEqual("456", dataSource.Select("Int32Setting"));

            //dataSource.Update("Int32Setting", "789");
            //Assert.AreEqual("789", dataSource.Select("Int32Setting"));
        }
    }

    [TestClass]
    public class DbSource_ctor
    {
        [TestMethod]
        public void connectionString_MustNotBeNull()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new DbSource<Setting>(null, null);
            }, ex => Assert.AreEqual("connectionString", ex.ParamName), Assert.Fail);
        }

        [TestMethod]
        public void settingTableName_MustNotBeNull()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new DbSource<Setting>("abc", null);
            }, ex => Assert.AreEqual("settingsTableName", ex.ParamName), Assert.Fail);
        }

        //[TestMethod]
        //public void customKeys_MustMatchTSetting()
        //{
        //    ExceptionAssert.Throws<SettingCustomKeysMismatchException>(() =>
        //    {
        //        var x = new DbSource<TestSetting>("abc", "xyz", new SettingKey[]
        //        {
        //            new SettingKey("Key1", "Value1"),
        //        });
        //    }, ex => Assert.AreEqual("Key1 = \"Value1\"", ex.CustomKeys), Assert.Fail);
        //}
    }

    [TestClass]
    public class DbSource_Select
    {
        private const string TestTableName = "TestSetting";
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            var connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;
            using (var context = new SmartConfigContext<TestSetting>(connectionString, TestTableName))
            {
                context.Database.Initialize(true);
            }
        }

        [TestMethod]
        public void CanSelectSettingsByName()
        {
            var dataSource = new DbSource<Setting>(_connectionString, TestTableName);
            Assert.AreEqual("123", dataSource.Select(new[] { new SettingKey(Setting.DefaultKeyName, "Int32Setting") }));
        }

        [TestMethod]
        public void CanSelectSettingsByNameByEnvironmentByVersion()
        {
            var dataSource = new DbSource<TestSetting>(_connectionString, TestTableName);
            var keys = new[]
            {
                new SettingKey(Setting.DefaultKeyName, "Int32Setting"),
                new SettingKey("Environment", "ABC"),
                new SettingKey("Version", "1.2.1"),
            };
            Assert.AreEqual("123", dataSource.Select(keys));
        }
    }
}
