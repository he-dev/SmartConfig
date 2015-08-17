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
            using (var context = new SmartConfigEntities<TestConfigElement>(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString, "TestConfig"))
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
            }
            .AddCustomKey(k => k.HasName(KeyNames.EnvironmentKeyName).HasValue("ABC").HasFilter(Filters.FilterByString))
            .AddCustomKey(k => k.HasName(KeyNames.VersionKeyName).HasValue("1.3.0").HasFilter(Filters.FilterByVersion));

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
                CustomKeys = new[]
                {
                    new CustomKey<TestConfigElement>()
                    {
                        Name = KeyNames.EnvironmentKeyName,
                        Value = "ABC",
                        Filter = Filters.FilterByString
                    },
                    new CustomKey<TestConfigElement>()
                    {
                        Name = KeyNames.VersionKeyName,
                        Filter = Filters.FilterByVersion
                    }
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
