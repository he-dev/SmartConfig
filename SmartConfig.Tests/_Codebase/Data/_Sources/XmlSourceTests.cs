using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Logging;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class XmlSourceTests
    {
        #region select tests

        [TestMethod]
        public void Select_Setting1()
        {
            var keys = new[]
            {
                new SettingKey(Setting.DefaultKeyName, "Setting1"),
            };

            var dataSource = new XmlSource<Setting>(@"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml");
            var value = dataSource.Select(keys);
            Assert.AreEqual("Value1", value);
        }

        [TestMethod]
        public void Select_Setting2_JKL()
        {
            var dataSource = new XmlSource<TestSetting>(@"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml");

            var keys = new[]
            {
                new SettingKey(Setting.DefaultKeyName, "Setting2"),
                new SettingKey("Environment", "JKL"),
                new SettingKey("Version", "1.2.1"),
            };

            var value = dataSource.Select(keys);
            Assert.AreEqual("Value2-JKL", value);
        }

        [TestMethod]
        public void Select_Setting2_ABC()
        {
            var dataSource = new XmlSource<TestSetting>(@"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml");

            var keys = new[]
            {
                new SettingKey(Setting.DefaultKeyName, "Setting2"),
                new SettingKey("Environment", "JKL"),
                new SettingKey("Version", "1.2.1"),
            };

            var value = dataSource.Select(keys);

            Assert.AreEqual("Value2-ABC", value);
        }

        [TestMethod]
        public void Select_Setting2_XYZ()
        {
            var dataSource = new XmlSource<TestSetting>(@"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml");

            var keys = new[]
            {
                new SettingKey(Setting.DefaultKeyName, "Setting2"),
                new SettingKey("Environment", "XYZ"),
                new SettingKey("Version", "1.2.1"),
            };

            var value = dataSource.Select(keys);
            Assert.AreEqual("Value2-XYZ", value);
        }

        [TestMethod]
        public void Select_Setting3()
        {
            var dataSource = new XmlSource<TestSetting>(@"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml");

            var keys = new[]
            {
                new SettingKey(Setting.DefaultKeyName, "Setting3"),
                new SettingKey("Environment", "ABC"),
                new SettingKey("Version", "1.2.1"),
            };
            var value = dataSource.Select(keys);
            Assert.AreEqual("Value6", value);
        }

        #endregion

        #region update tests

        [TestMethod]
        public void Update_Setting1()
        {
            var xmlSource = new XmlSource<Setting>(@"TestFiles\XmlConfigs\XmlConfig_UpdateTests.xml");

            var keys = new[]
            {
                new SettingKey(Setting.DefaultKeyName, "Setting1"),
            };

            var oldValue = xmlSource.Select(keys);
            Assert.AreEqual("Value1", oldValue);

            //xmlSource.Update("Setting1", "Value2");
            //var newValue = xmlSource.Select("Setting1");
            //Assert.AreEqual("Value2", newValue);
        }

        [TestMethod]
        public void Update_Setting2_new()
        {
            Logger.Warn = m => Debug.WriteLine(m);

            var xmlSource = new XmlSource<Setting>(@"TestFiles\XmlConfigs\XmlConfig_UpdateTests.xml");

            var keys = new[]
            {
                new SettingKey(Setting.DefaultKeyName, "Setting2"),
            };

            var oldValue = xmlSource.Select(keys);
            Assert.AreEqual(null, oldValue);

            //xmlSource.Update("Setting2", "Value2");
            //var newValue = xmlSource.Select("Setting2");
            //Assert.AreEqual("Value2", newValue);
        }

        #endregion

        #region other tests

        [TestMethod]
        public void EncodeKeyName_CamelCase()
        {
            //Assert.AreEqual("camel-case", XmlSource<TestSetting>.EncodeKeyName("CamelCase"));
        }

     
        #endregion

    }
}
