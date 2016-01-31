using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Paths;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.Data.XmlSourceTests
{
    //[TestClass]
    public class XmlSourceTestsBase
    {
        protected const string TestFileName =
              @"C:\Home\Projects\SmartConfig\SmartConfig.Tests\bin\Debug\TestFiles\XmlConfigs\TestXmlConfig.xml";
    }

    [TestClass]
    public class ctor : XmlSourceTestsBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequiresFileName()
        {
            new XmlSource<Setting>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNameNotRootedException))]
        public void RequiresFileNameIsRooted()
        {
            new XmlSource<Setting>(@"a\b\c.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void RequiresFileNameExists()
        {
            new XmlSource<Setting>(@"C:\a\b\c.xml");
        }
    }

    [TestClass]
    public class Select : XmlSourceTestsBase
    {
        [TestMethod]
        public void SelectsSettingByName()
        {
            var dataSource = new XmlSource<Setting>(TestFileName);
            var value = dataSource.Select(new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "Setting1")),
                Enumerable.Empty<SettingKey>()));
            Assert.AreEqual("Value1", value);
        }

        [TestMethod]
        public void SelectsSettingByNameByEnvironmentByVersion()
        {
            var dataSource = new XmlSource<TestSetting>(TestFileName);

            var keys = new SettingKeyCollection(
                new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "Setting2")),
                new[]
                {
                    new SettingKey("Environment", "JKL"),
                    new SettingKey("Version", "1.2.1")
                });

            var value = dataSource.Select(keys);
            Assert.AreEqual("Value2-JKL", value);
        }

    }

    [TestClass]
    public class Update : XmlSourceTestsBase
    {

        [TestMethod]
        public void UpdatesSettingByName()
        {
            var xmlSource = new XmlSource<Setting>(TestFileName);
            
            var keys = new SettingKeyCollection(new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "Setting1")), Enumerable.Empty<SettingKey>());

            var oldValue = xmlSource.Select(keys);
            Assert.AreEqual("Value1", oldValue);

            xmlSource.Update(keys, "Value2");
            var newValue = xmlSource.Select(keys);
            Assert.AreEqual("Value2", newValue);
        }

        [TestMethod]
        public void AddsNewSetting()
        {
            //Logger.Warn = m => Debug.WriteLine(m);

            var xmlSource = new XmlSource<Setting>(TestFileName);

            var keys = new SettingKeyCollection(new SettingKey(Setting.DefaultKeyName, new SettingPath(null, "NewSetting")), Enumerable.Empty<SettingKey>());

            var oldValue = xmlSource.Select(keys);
            Assert.AreEqual(null, oldValue);

            xmlSource.Update(keys, "NewValue");
            var newValue = xmlSource.Select(keys);
            Assert.AreEqual("NewValue", newValue);
        }

    }
}
