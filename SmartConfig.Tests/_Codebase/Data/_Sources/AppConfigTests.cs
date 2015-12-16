using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class AppConfigTests
    {
        [TestMethod]
        public void Select_CanSelectAppSetting()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select(new SettingKeyReadOnlyCollection(new SettingKey(Setting.DefaultKeyName, "AppSettings", "TestSetting")));
            Assert.AreEqual("TestValue", value);
        }

        [TestMethod]
        public void Select_CanSelectAppSettingWithCustomName()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select(new SettingKeyReadOnlyCollection(new SettingKey(Setting.DefaultKeyName, "myApp", "AppSettings", "TestSetting")));
            Assert.AreEqual("MyAppTestValue", value);
        }

        [TestMethod]
        public void Select_CanSelectConnectionString()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select(new SettingKeyReadOnlyCollection(new SettingKey(Setting.DefaultKeyName, "ConnectionStrings", "TestSetting")));
            Assert.AreEqual("TestConnectionString", value);
        }

        [TestMethod]
        public void Select_CanSelectConnectionStringWithCusomName()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select(new SettingKeyReadOnlyCollection(new SettingKey(Setting.DefaultKeyName, "myApp", "ConnectionStrings", "TestSetting")));
            Assert.AreEqual("MyAppTestConnectionString", value);
        }

        [TestMethod]
        public void Update_CanUpdateSettingByName()
        {
            var appConfigSource = new AppConfigSource();
            appConfigSource.Update(new SettingKeyReadOnlyCollection(new SettingKey(Setting.DefaultKeyName, "myApp", "AppSettings", "TestSetting")), "NewValue");

            var value = appConfigSource.Select(new SettingKeyReadOnlyCollection(new SettingKey(Setting.DefaultKeyName, "myApp", "AppSettings", "TestSetting")));
            Assert.AreEqual("NewValue", value);
        }
    }
}
