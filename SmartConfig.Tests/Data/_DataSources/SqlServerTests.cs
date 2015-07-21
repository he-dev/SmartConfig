using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class SqlServerTests
    {
        public TestContext TextContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            using (var context = new global::SmartConfig.Data.SmartConfigEntities(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString, "TestConfig"))
            {
                context.Database.Initialize(true);
            }
        }

        [TestMethod]
        public void Select_TestExistingName()
        {
            var sqlServer = new global::SmartConfig.Data.SqlServer()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString,
                TableName = "TestConfig"
            };
            var configElement = sqlServer.Select("ABCD.Int32Field").SingleOrDefault();
            Assert.IsNotNull(configElement);
            Assert.AreEqual<string>("ABC", configElement.Environment);
            Assert.AreEqual<string>("1.0.0", configElement.Version);
            Assert.AreEqual<string>("ABCD.Int32Field", configElement.Name);
            Assert.AreEqual<string>("2", configElement.Value);
        }
    }
}
