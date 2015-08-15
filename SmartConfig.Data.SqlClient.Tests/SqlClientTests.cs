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
            using (var context = new SmartConfigEntities<TestConfigElement>(
                ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                "TestConfig",
                new[] { KeyNames.EnvironmentKeyName, KeyNames.VersionKeyName }))
            {
                context.Database.Initialize(true);
            }
            //Database.SetInitializer<SmartConfigEntities>(null);
        }

        [TestMethod]
        public void Select_Int32Field()
        {
            var dataSource = new SqlClient<TestConfigElement>()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                TableName = "TestConfig",
                Keys = new Dictionary<string, string>() { { KeyNames.EnvironmentKeyName, "ABC" } },
                Filters = new Dictionary<string, FilterByFunc<TestConfigElement>>()
                {
                    { KeyNames.EnvironmentKeyName, Filters.FilterByString },
                    { KeyNames.VersionKeyName, Filters.FilterByVersion }
                }
            };
            SmartConfigManager.Load(typeof(BasicConfig), dataSource);
            Assert.AreEqual(123, BasicConfig.Int32Field);
        }

        [TestMethod]
        public void Update_Int32Fields()
        {
            var dataSource = new SqlClient<TestConfigElement>()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                TableName = "TestConfig",
                Keys = new Dictionary<string, string>() { { KeyNames.EnvironmentKeyName, "ABC" } },
                Filters = new Dictionary<string, FilterByFunc<TestConfigElement>>()
                {
                    { KeyNames.EnvironmentKeyName, Filters.FilterByString },
                    { KeyNames.VersionKeyName, Filters.FilterByVersion }
                }
            };
            SmartConfigManager.Load(typeof(BasicConfig), dataSource);

            Assert.AreEqual(123, BasicConfig.Int32Field);

            SmartConfigManager.Update(() => BasicConfig.Int32Field, 456);
            Assert.AreEqual("456", dataSource.Select(new Dictionary<string, string>()
            {
                { KeyNames.DefaultKeyName, "Int32Field" },
                { KeyNames.VersionKeyName, "1.3.0" }
            }));
            Assert.AreEqual(456, BasicConfig.Int32Field);

            SmartConfigManager.Update(() => BasicConfig.Int32Field, 789);
            Assert.AreEqual("789", dataSource.Select(new Dictionary<string, string>()
            {
                { KeyNames.DefaultKeyName, "Int32Field" },
                { KeyNames.VersionKeyName, "1.3.0" }
            }));
            Assert.AreEqual(789, BasicConfig.Int32Field);
        }
    }
}
