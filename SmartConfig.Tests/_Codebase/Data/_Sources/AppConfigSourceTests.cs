using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Paths;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class AppConfigSourceTests
    {
        [TestMethod]
        public void SelectsAppSettingByKey()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select(
                new SettingKeyCollection(
                    defaultKey: new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "AppSettings", "AppConfigSourceTests", "Setting1")),
                    customKeys: Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("foo", value);
        }

        [TestMethod]
        public void SelectAppSettingByKeyAndConfigName()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select(
                new SettingKeyCollection(
                    defaultKey: new SettingKey(Setting.DefaultKeyName, new SettingPath("baz", "AppSettings", "AppConfigSourceTests", "Setting2")),
                    customKeys: Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("bar", value);
        }

        [TestMethod]
        public void SelectsonnectionStringByName()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select(new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "ConnectionStrings", "AppConfigSourceTests", "ConnectionString1")), 
                Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("qux", value);
        }

        [TestMethod]
        public void SelectsonnectionStringByNameAndConfigName()
        {
            var appConfigSource = new AppConfigSource();
            var value = appConfigSource.Select(new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, new SettingPath("baz", "ConnectionStrings", "AppConfigSourceTests", "ConnectionString2")),
                Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("quux", value);
        }

        [TestMethod]
        public void UpdatesAppSettingByKeyAndConfigName()
        {
            var appConfigSource1 = new AppConfigSource();
            appConfigSource1.Update(
                new SettingKeyCollection(
                    new SettingKey(Setting.DefaultKeyName, new SettingPath("baz", "AppSettings", "AppConfigSourceTests", "Setting3")), 
                    Enumerable.Empty<SettingKey>()
                ),
                "quux"
            );

            var appConfigSource2 = new AppConfigSource();
            var newValue = appConfigSource2.Select(
                new SettingKeyCollection(
                    new SettingKey(Setting.DefaultKeyName, new SettingPath("baz", "AppSettings", "AppConfigSourceTests", "Setting3")),
                    Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("quux", newValue);
        }
    }
}
