using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace SmartConfig.DataStores.AppConfig.Tests.AppConfigPathTests
{
    [TestClass]
    public class ctor
    {
        [TestMethod]
        public void CreatesAppConfigPathWithoutConfigurationName()
        {
            var appConfigPath = new AppConfigPath(new SettingPath(null, "foo", "bar"));
            Assert.AreEqual("foo", appConfigPath.SectionName);
            Assert.AreEqual("bar", appConfigPath);
        }

        [TestMethod]
        public void CreatesAppConfigPathWithConfigurationName()
        {
            var appConfigPath = new AppConfigPath(new SettingPath("foo", "bar", "baz"));
            Assert.AreEqual("bar", appConfigPath.SectionName);
            Assert.AreEqual("foo.baz", appConfigPath);
        }
    }
}
