using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Filters;
using SmartConfig.Tests.TestConfigs;
using SmartUtilities.UnitTesting;
// ReSharper disable InconsistentNaming

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class DbSourceTests
    {
        protected const string _testTableName = "TestSetting";
        protected const string _connectionStringName = "TestDb";
        protected readonly string _connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString;

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            var connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString;
            using (var context = new SmartConfigDbContext<TestSetting>(connectionString, _testTableName))
            {
                context.Database.Initialize(true);
            }
        }

        [TestMethod]
        public void RequiresConnectionString()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new DbSource<Setting>(null, null);
            }, ex => Assert.AreEqual("connectionString", ex.ParamName), Assert.Fail);
        }

        [TestMethod]
        public void RequiresTableName()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new DbSource<Setting>("abc", null);
            }, ex => Assert.AreEqual("settingsTableName", ex.ParamName), Assert.Fail);
        }

        [TestMethod]
        public void SelectsSettingByName()
        {
            var dataSource = new DbSource<Setting>(_connectionString, _testTableName);
            var value = dataSource.Select(
                new SettingKeyCollection(
                    new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "Int32Setting")),
                    Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("123", value);
        }

        [TestMethod]
        public void SelectsSettingByNameByEnvironmentByVersion()
        {
            var dataSource = new DbSource<TestSetting>(_connectionString, _testTableName);
            var keys = new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "Int32Setting")),
                new[]
                {
                    new SettingKey("Environment", "A"),
                    new SettingKey("Version", "2.2.1")
                });
            Assert.AreEqual("123", dataSource.Select(keys));
        }

        [TestMethod]
        public void UpdatesSettingByName()
        {
            var dataSource = new DbSource<Setting>(_connectionString, _testTableName);

            var keys = new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "Int32Setting")), 
                Enumerable.Empty<SettingKey>());

            Assert.AreEqual("123", dataSource.Select(keys));

            dataSource.Update(keys, "456");
            Assert.AreEqual("456", dataSource.Select(keys));

            dataSource.Update(keys, "789");
            Assert.AreEqual("789", dataSource.Select(keys));
        }

        // --- test full configs

        [TestMethod]
        public void LoadsSettingsFromDatabase()
        {
            Configuration.LoadSettings(typeof(DatabaseSettings1));

            Assert.AreEqual("baz", DatabaseSettings1.StringSetting);
            Assert.AreEqual("123", DatabaseSettings1.Int32Setting);

            Configuration.LoadSettings(typeof(DatabaseSettings2));

            Assert.AreEqual("qux", DatabaseSettings2.StringSetting);
            Assert.AreEqual("890", DatabaseSettings2.Int32Setting);
        }
    }
}
