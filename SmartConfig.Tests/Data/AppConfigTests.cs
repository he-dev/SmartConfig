using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class AppConfigTests
    {
        [TestMethod]
        public void Load_AppConfig()
        {
            SmartConfigManager.Load(typeof(TestConfigs.AppConfig), new Data.AppConfig());
            Assert.AreEqual("TestValue", TestConfigs.AppConfig.AppSettings.TestKey);
            Assert.AreEqual("TestConnectionString", TestConfigs.AppConfig.ConnectionStrings.TestName);
        }
    }
}
