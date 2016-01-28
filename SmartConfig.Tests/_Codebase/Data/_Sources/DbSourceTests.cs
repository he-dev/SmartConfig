using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        private const string _testTableName = "TestSetting";
        private const string _connectionStringName = "TestDb";
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            var connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString;
            using (var context = new SmartConfigContext<TestSetting>(connectionString, _testTableName))
            {
                context.Database.Initialize(true);
            }
        }

        [TestMethod]
        public void ctor_connectionString_MustNotBeNull()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new DbSource<Setting>(null, null);
            }, ex => Assert.AreEqual("connectionString", ex.ParamName), Assert.Fail);
        }

        [TestMethod]
        public void ctor_settingTableName_MustNotBeNull()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new DbSource<Setting>("abc", null);
            }, ex => Assert.AreEqual("settingsTableName", ex.ParamName), Assert.Fail);
        }

        [TestMethod]
        public void Select_CanSelectSettingsByName()
        {
            var dataSource = new DbSource<Setting>(_connectionString, _testTableName);
            Assert.AreEqual("123", dataSource.Select(new SettingKeyCollection(new SettingKey(Setting.DefaultKeyName, "Int32Setting"), Enumerable.Empty<SettingKey>())));
        }

        [TestMethod]
        public void Select_CanSelectSettingsByNameByEnvironmentByVersion()
        {
            var dataSource = new DbSource<TestSetting>(_connectionString, _testTableName);
            var keys = new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, "Int32Setting"),
                new [] 
                {
                    new SettingKey("Environment", "ABC"),
                    new SettingKey("Version", "1.2.1")
                });
            Assert.AreEqual("123", dataSource.Select(keys));
        }

        [TestMethod]
        public void Update_CanUpdateSettingByName()
        {
            var dataSource = new DbSource<Setting>(_connectionString, _testTableName);

            var keys = new SettingKeyCollection(new SettingKey(Setting.DefaultKeyName, "Int32Setting"), Enumerable.Empty<SettingKey>());

            Assert.AreEqual("123", dataSource.Select(keys));

            dataSource.Update(keys, "456");
            Assert.AreEqual("456", dataSource.Select(keys));

            dataSource.Update(keys, "789");
            Assert.AreEqual("789", dataSource.Select(keys));
        }
    }
}
