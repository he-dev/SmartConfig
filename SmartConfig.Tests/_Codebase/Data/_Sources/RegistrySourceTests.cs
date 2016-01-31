using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Paths;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class RegistrySourceTests
    {
        [TestMethod]
        public void RequiresBaseRegistryKey()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new RegistrySource<Setting>(null, null);
            }, null, Assert.Fail);
        }

        [TestMethod]
        public void RequiresRegistrySubKey()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                new RegistrySource<Setting>(Registry.CurrentUser, null);
            }, null, Assert.Fail);
        }

        [TestMethod]
        public void SelectsStringSettingByName()
        {
            var registrySource = new RegistrySource<Setting>(
                Registry.CurrentUser,
                @"software\he-dev\smartconfig.tests");

            var value = registrySource.Select(
                new SettingKeyCollection(
                    new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "StringSetting")),
                    Enumerable.Empty<SettingKey>()
                )
            );

            Assert.AreEqual("foo", value);
        }

        [TestMethod]
        public void SelectsInt32SettingByName()
        {
            var registrySource = new RegistrySource<Setting>(
                Registry.CurrentUser,
                @"software\he-dev\smartconfig.tests");

            var value = registrySource.Select(
                new SettingKeyCollection(
                    new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "Int32Setting")),
                    Enumerable.Empty<SettingKey>()
                )
            );

            Assert.AreEqual(123, value);
        }

        [TestMethod]
        public void SelectsByteSettingByName()
        {

        }

        [TestMethod]
        public void UpdatesStringSettingByName()
        {
            var registrySource = new RegistrySource<Setting>(
                Registry.CurrentUser,
                @"software\he-dev\smartconfig.tests");

            var settingKeyCollection = new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "StringSetting2")),
                Enumerable.Empty<SettingKey>());

            var newValue = DateTime.Now.Second;

            registrySource.Update(settingKeyCollection, newValue.ToString());

            var value = registrySource.Select(settingKeyCollection);

            Assert.AreEqual(newValue.ToString(), value);
        }
    }
}
