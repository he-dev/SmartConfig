using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class AppConfigTests
    {        
        [TestMethod]
        public void Select_AppSetting()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select("AppSettings.TestSetting");
            Assert.AreEqual("TestValue", value);
        }

        [TestMethod]
        public void Select_ConnectionString()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select("ConnectionStrings.TestSetting");
            Assert.AreEqual("TestConnectionString", value);
        }
    }
}
