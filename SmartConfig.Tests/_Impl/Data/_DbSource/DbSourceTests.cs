using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.Tests.TestConfigs;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class SqlClientTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            var connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;
            using (var context = new SmartConfigContext<TestSetting>(connectionString)
            {
                SettingsTableName = "TestConfig",
                SettingsTableKeyNames = KeyNames.From<TestSetting>()
            })
            {
                context.Database.Initialize(true);
            }
        }

        [TestMethod]
        public void Select_Int32Field()
        {
            var dataSource = new DbSource<TestSetting>()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                SettingTableName = "TestConfig",
                KeyProperties = new Dictionary<string, KeyProperties>()
                {
                    { KeyNames.EnvironmentKeyName, new KeyProperties() { Value = "ABC", Filter = Filters.FilterByString }},
                    { KeyNames.VersionKeyName, new KeyProperties() { Value = "1.3.0", Filter = Filters.FilterByVersion }},
                }
            };

            SmartConfigManager.Load(typeof(SimpteTestConfig), dataSource);
            Assert.AreEqual(123, SimpteTestConfig.Int32Field);
        }

        [TestMethod]
        public void Update_Int32Fields()
        {
            var dataSource = new DbSource<TestSetting>()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                SettingTableName = "TestConfig",
                KeyProperties = new Dictionary<string, KeyProperties>()
                {
                    { KeyNames.EnvironmentKeyName, new KeyProperties() { Value = "ABC", Filter = Filters.FilterByString }},
                    { KeyNames.VersionKeyName, new KeyProperties() { Value = "1.3.0", Filter = Filters.FilterByVersion }},
                }
            };

            SmartConfigManager.Load(typeof(SimpteTestConfig), dataSource);

            Assert.AreEqual(123, SimpteTestConfig.Int32Field);

            SmartConfigManager.Update(() => SimpteTestConfig.Int32Field, 456);
            Assert.AreEqual("456", dataSource.Select("Int32Field"));
            Assert.AreEqual(456, SimpteTestConfig.Int32Field);

            SmartConfigManager.Update(() => SimpteTestConfig.Int32Field, 789);
            Assert.AreEqual("789", dataSource.Select("Int32Field"));
            Assert.AreEqual(789, SimpteTestConfig.Int32Field);
        }
    }
}
