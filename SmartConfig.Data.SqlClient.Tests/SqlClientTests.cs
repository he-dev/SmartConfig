﻿using System;
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
            using (var context = new SmartConfigEntities<TestSetting>(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString)
            {
                SettingsTableName = "TestConfig"
            })
            {
                context.Database.Initialize(true);
            }
            //Database.SetInitializer<SmartConfigEntities>(null);
        }

        [TestMethod]
        public void Select_Int32Field()
        {
            var dataSource = new SqlClient<TestSetting>()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                SettingTableName = "TestConfig",
            }
            .ConfigureKey(KeyNames.EnvironmentKeyName, k =>
            {
                k.Value = "ABC";
                k.Filter = Filters.FilterByString;
            })
            .ConfigureKey(KeyNames.VersionKeyName, k =>
            {
                k.Value = "1.3.0";
                k.Filter = Filters.FilterByVersion;
            });

            SmartConfigManager.Load(typeof(BasicConfig), dataSource);
            Assert.AreEqual(123, BasicConfig.Int32Field);
        }

        [TestMethod]
        public void Update_Int32Fields()
        {
            var dataSource = new SqlClient<TestSetting>()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                SettingTableName = "TestConfig"
            }
            .ConfigureKey(KeyNames.EnvironmentKeyName, k =>
            {
                k.Value = "ABC";
                k.Filter = Filters.FilterByString;
            })
            .ConfigureKey(KeyNames.VersionKeyName, k =>
            {
                k.Value = "1.3.0";
                k.Filter = Filters.FilterByVersion;
            });

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
