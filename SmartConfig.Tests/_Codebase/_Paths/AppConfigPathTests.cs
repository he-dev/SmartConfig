using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class AppConfigPathTests
    {
        [TestMethod]
        public void GetsPathWithoutConfigNameSkippingSectionName()
        {
            var appConfigPath = new AppConfigPath(new SettingPath(null, "AppSettings", "TestSetting"));
            Assert.AreEqual("AppSettings", appConfigPath.SectionName);
            Assert.AreEqual("TestSetting", appConfigPath);
        }

        [TestMethod]
        public void GetsPathWithoutWithConfigNameSkippingSectionName()
        {
            var appConfigPath = new AppConfigPath(new SettingPath("testconfig", "AppSettings", "TestSetting"));
            Assert.AreEqual("AppSettings", appConfigPath.SectionName);
            Assert.AreEqual("testconfig.TestSetting", appConfigPath);
        }
    }
}
