using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class AppConfigTests
    {
        //[TestMethod]
        //public void Load_AppConfig()
        //{
        //    Configuration.LoadSettings(typeof(TestConfigs.AppConfig), new SmartConfig.Data.AppConfigSource());
        //    Assert.AreEqual("TestValue", TestConfigs.AppConfig.AppSettings.TestKey);
        //    Assert.AreEqual("TestConnectionString", TestConfigs.AppConfig.ConnectionStrings.TestName);
        //}

        [TestMethod]
        public void Select_AppSetting()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select("appsettings.testkey");
            Assert.AreEqual("TestValue", value);
        }

        [TestMethod]
        public void Select_ConnectionString()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select("connectionstrings.testname");
            Assert.AreEqual("TestConnectionString", value);
        }
    }
}
