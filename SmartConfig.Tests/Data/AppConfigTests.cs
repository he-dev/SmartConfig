using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class AppConfigTests
    {
        [TestMethod]
        public void Load_AppConfig()
        {
            SmartConfigManager.Load(typeof(TestConfigs.AppConfig), new SmartConfig.Data.AppConfigSource());
            Assert.AreEqual("TestValue", TestConfigs.AppConfig.AppSettings.TestKey);
            Assert.AreEqual("TestConnectionString", TestConfigs.AppConfig.ConnectionStrings.TestName);
        }
    }
}
