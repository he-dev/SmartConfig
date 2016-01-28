using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Logging;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class XmlSourceTests
    {
        private const string TestFileName = @"C:\Home\Projects\SmartConfig\SmartConfig.Tests\bin\Debug\TestFiles\XmlConfigs\TestXmlConfig.xml";

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ctor_fileName_MustNotBeNull()
        {
            new XmlSource<Setting>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNameNotRootedException))]
        public void ctor_fileName_MustBeRooted()
        {
            new XmlSource<Setting>(@"a\b\c.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ctor_fileName_MustExist()
        {
            new XmlSource<Setting>(@"C:\a\b\c.xml");
        }

        [TestMethod]
        public void Select_CanSelectSettingByName()
        {
            var dataSource = new XmlSource<Setting>(TestFileName);
            var value = dataSource.Select(new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "Setting1")), 
                Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("Value1", value);
        }

        [TestMethod]
        public void Select_CanSelectSettingByNameByEnvironmentByVersion()
        {
            var dataSource = new XmlSource<TestSetting>(TestFileName);

            var keys = new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, "Setting2"),
                new [] 
                {
                    new SettingKey("Environment", "JKL"),
                    new SettingKey("Version", "1.2.1")
                });

            var value = dataSource.Select(keys);
            Assert.AreEqual("Value2-JKL", value);
        }

        [TestMethod]
        public void Update_CanUpdateSettingByName()
        {
            var xmlSource = new XmlSource<Setting>(TestFileName);

            var keys = new SettingKeyCollection(new SettingKey(Setting.DefaultKeyName, "Setting1"), Enumerable.Empty<SettingKey>());

            var oldValue = xmlSource.Select(keys);
            Assert.AreEqual("Value1", oldValue);

            xmlSource.Update(keys, "Value2");
            var newValue = xmlSource.Select(keys);
            Assert.AreEqual("Value2", newValue);
        }

        [TestMethod]
        public void Update_CanAddNewSetting()
        {
            //Logger.Warn = m => Debug.WriteLine(m);

            var xmlSource = new XmlSource<Setting>(TestFileName);

            var keys = new SettingKeyCollection(new SettingKey(Setting.DefaultKeyName, "NewSetting"), Enumerable.Empty<SettingKey>());

            var oldValue = xmlSource.Select(keys);
            Assert.AreEqual(null, oldValue);

            xmlSource.Update(keys, "NewValue");
            var newValue = xmlSource.Select(keys);
            Assert.AreEqual("NewValue", newValue);
        }

    }
}
