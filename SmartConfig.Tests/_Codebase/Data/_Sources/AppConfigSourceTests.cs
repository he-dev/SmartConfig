using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class AppConfigSourceTests
    {
        [TestMethod]
        public void Select_CanSelectAppSetting()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select(new SettingKeyCollection(
                defaultKey: new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "AppSettings", "TestSetting")),
                customKeys: Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("TestValue", value);
        }

        [TestMethod]
        public void Select_CanSelectAppSettingWithCustomName()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select(new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, new SettingPath("myApp", "AppSettings", "TestSetting")), 
                Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("MyAppTestValue", value);
        }

        [TestMethod]
        public void Select_CanSelectConnectionString()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select(new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "ConnectionStrings", "TestSetting")), 
                Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("TestConnectionString", value);
        }

        [TestMethod]
        public void Select_CanSelectConnectionStringWithCusomName()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select(new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, new SettingPath("myApp", "ConnectionStrings", "TestSetting")),
                Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("MyAppTestConnectionString", value);
        }

        [TestMethod]
        public void Update_CanUpdateSettingByName()
        {
            var appConfigSource = new AppConfigSource();
            appConfigSource.Update(
                new SettingKeyCollection(
                    new SettingKey(Setting.DefaultKeyName, 
                    new SettingPath("myApp", "AppSettings", "TestSetting")), Enumerable.Empty<SettingKey>()
                ),
                "NewValue"
            );

            var value = appConfigSource.Select(new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, new SettingPath("myApp", "AppSettings", "TestSetting")),
                Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("NewValue", value);
        }
    }
}
