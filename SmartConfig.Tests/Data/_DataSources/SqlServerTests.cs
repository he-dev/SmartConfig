using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class SqlServerTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            using (var context = new SmartConfigEntities(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString, "TestConfig"))
            {
                context.Database.Initialize(true);
            }
        }

        [TestMethod]
        public void Select_Values()
        {
            var sqlServer = new SqlServer()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                TableName = "TestConfig"
            };
            var configElements = sqlServer.Select("ABC", "2.2.0", "StringField").ToList();
            Assert.AreEqual(1, configElements.Count);
        }

        [TestMethod]
        public void Update_ExistingConfigElement()
        {
            var sqlServer = new SqlServer()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                TableName = "TestConfig"
            };
            sqlServer.Update(new ConfigElement()
            {
                Environment = "JKL",
                Version = "3.2.4",
                Name = "StringField",
                Value = "jkl"
            });
            var configElements = sqlServer.Select("JKL", "3.2.4", "StringField").ToList();
            Assert.AreEqual(1, configElements.Count);
            Assert.AreEqual("jkl", configElements[0].Value);
        }
    }
}
