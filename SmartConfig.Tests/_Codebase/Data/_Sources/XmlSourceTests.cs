using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var value = new XmlSource<Setting>(@"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml")
            .Select("Setting1");
            Assert.AreEqual("Value1", value);
        }

        [TestMethod]
        public void Select_Setting2_JKL()
        {
            var value = new XmlSource<TestSetting>(@"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml", new[]
                {
                    new CustomKey(KeyNames.EnvironmentKeyName, "JKL", Filters.FilterByString),
                    new CustomKey(KeyNames.VersionKeyName, "1.2.1", Filters.FilterByVersion)
                })
            .Select("Setting2");
            Assert.AreEqual("Value2-JKL", value);
        }

        [TestMethod]
        public void Select_Setting2_ABC()
        {
            var value = new XmlSource<TestSetting>(@"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml", new[]
                {
                    new CustomKey(KeyNames.EnvironmentKeyName, "ABC", Filters.FilterByString),
                    new CustomKey(KeyNames.VersionKeyName, "1.2.1", Filters.FilterByVersion)
                })
            .Select("Setting2");
            Assert.AreEqual("Value2-ABC", value);
        }

        [TestMethod]
        public void Select_Setting2_XYZ()
        {
            var value = new XmlSource<TestSetting>(@"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml", new[]
                {
                    new CustomKey(KeyNames.EnvironmentKeyName, "XYZ", Filters.FilterByString),
                    new CustomKey(KeyNames.VersionKeyName, "1.2.1", Filters.FilterByVersion)
                })
            .Select("Setting2");
            Assert.AreEqual("Value2-XYZ", value);
        }

        [TestMethod]
        public void Select_Setting3()
        {
            var value = new XmlSource<TestSetting>(@"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml", new[]
                {
                    new CustomKey(KeyNames.EnvironmentKeyName, "ABC", Filters.FilterByString),
                    new CustomKey(KeyNames.VersionKeyName, "1.2.1", Filters.FilterByVersion)
                })
            .Select("Setting3");
            Assert.AreEqual("Value6", value);
        }

        #endregion

        #region update tests

        [TestMethod]
        public void Update_Setting1()
        {
            var xmlSource = new XmlSource<Setting>(@"TestFiles\XmlConfigs\XmlConfig_UpdateTests.xml");

            var oldValue = xmlSource.Select("Setting1");
            Assert.AreEqual("Value1", oldValue);

            xmlSource.Update("Setting1", "Value2");
            var newValue = xmlSource.Select("Setting1");
            Assert.AreEqual("Value2", newValue);
        }

        [TestMethod]
        public void Update_Setting2_new()
        {
            Logger.Warn = m => Debug.WriteLine(m);

            var xmlSource = new XmlSource<Setting>(@"TestFiles\XmlConfigs\XmlConfig_UpdateTests.xml");

            var oldValue = xmlSource.Select("Setting2");
            Assert.AreEqual(null, oldValue);

            xmlSource.Update("Setting2", "Value2");
            var newValue = xmlSource.Select("Setting2");
            Assert.AreEqual("Value2", newValue);
        }

        #endregion

        #region other tests

        [TestMethod]
        public void EncodeKeyName_CamelCase()
        {
            Assert.AreEqual("camel-case", XmlSource<TestSetting>.EncodeKeyName("CamelCase"));
        }

     
        #endregion

    }
}
