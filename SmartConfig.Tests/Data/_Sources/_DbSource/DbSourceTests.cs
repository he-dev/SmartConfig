using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.Data.SqlClient.Tests;

namespace SmartConfig.Data.SqlClient.Tests
{
    [TestClass]
    public class SqlClientTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            // TODO: define key columns
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            using (var context = new SmartConfigContext<TestSetting>(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString)
            {
                SettingsTableName = "TestConfig"
            })
            {
                context.Database.Initialize(true);
            }
            //Database.SetInitializer<SmartConfigContext>(null);
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

            SmartConfigManager.Load(typeof(BasicConfig), dataSource);
            Assert.AreEqual(123, BasicConfig.Int32Field);
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

            SmartConfigManager.Load(typeof(BasicConfig), dataSource);

            Assert.AreEqual(123, BasicConfig.Int32Field);

            SmartConfigManager.Update(() => BasicConfig.Int32Field, 456);
            Assert.AreEqual("456", dataSource.Select("Int32Field"));
            Assert.AreEqual(456, BasicConfig.Int32Field);

            SmartConfigManager.Update(() => BasicConfig.Int32Field, 789);
            Assert.AreEqual("789", dataSource.Select("Int32Field"));
            Assert.AreEqual(789, BasicConfig.Int32Field);
        }
    }
}
