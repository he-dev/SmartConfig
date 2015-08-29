using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;

namespace SmartConfig.Tests.Data
{
    [TestClass]
    public class XmlSourceTests
    {
        #region select tests

        [TestMethod]
        public void Select_Setting1()
        {
            var value = new XmlSource<Setting>()
            {
                FileName = @"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml"
            }
            .Select("Setting1");
            Assert.AreEqual("Value1", value);
        }

        [TestMethod]
        public void Select_Setting2_JKL()
        {
            var value = new XmlSource<TestSetting>()
            {
                FileName = @"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml",
                KeyProperties = new Dictionary<string, KeyProperties>
                {
                    { KeyNames.EnvironmentKeyName, new KeyProperties() { Value = "JKL", Filter = Filters.FilterByString } },
                    { KeyNames.VersionKeyName, new KeyProperties() { Value = "1.2.1", Filter = Filters.FilterByVersion } }
                }
            }
            .Select("Setting2");
            Assert.AreEqual("Value2-JKL", value);
        }

        [TestMethod]
        public void Select_Setting2_ABC()
        {
            var value = new XmlSource<TestSetting>()
            {
                FileName = @"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml",
                KeyProperties = new Dictionary<string, KeyProperties>
                {
                    { KeyNames.EnvironmentKeyName, new KeyProperties() { Value = "ABC", Filter = Filters.FilterByString } },
                    { KeyNames.VersionKeyName, new KeyProperties() { Value = "1.2.1", Filter = Filters.FilterByVersion } }
                }
            }
            .Select("Setting2");
            Assert.AreEqual("Value2-ABC", value);
        }

        [TestMethod]
        public void Select_Setting2_XYZ()
        {
            var value = new XmlSource<TestSetting>()
            {
                FileName = @"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml",
                KeyProperties = new Dictionary<string, KeyProperties>
                {
                    { KeyNames.EnvironmentKeyName, new KeyProperties() { Value = "XYZ", Filter = Filters.FilterByString } },
                    { KeyNames.VersionKeyName, new KeyProperties() { Value = "1.2.1", Filter = Filters.FilterByVersion } }
                }
            }
            .Select("Setting2");
            Assert.AreEqual("Value2-XYZ", value);
        }

        [TestMethod]
        public void Select_Setting3()
        {
            var value = new XmlSource<TestSetting>()
            {
                FileName = @"TestFiles\XmlConfigs\XmlConfig_SelectTests.xml",
                KeyProperties = new Dictionary<string, KeyProperties>
                {
                    { KeyNames.EnvironmentKeyName, new KeyProperties() { Value = "ABC", Filter = Filters.FilterByString } },
                    { KeyNames.VersionKeyName, new KeyProperties() { Value = "1.2.1", Filter = Filters.FilterByVersion } }
                }
            }
            .Select("Setting3");
            Assert.AreEqual("Value6", value);
        }

        #endregion

        #region update tests

        [TestMethod]
        public void Update_Setting1()
        {
            var xmlSource = new XmlSource<Setting>()
            {
                FileName = @"TestFiles\XmlConfigs\XmlConfig_UpdateTests.xml"
            };

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

            var xmlSource = new XmlSource<Setting>()
            {
                FileName = @"TestFiles\XmlConfigs\XmlConfig_UpdateTests.xml"
            };

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

        [TestMethod]
        public void set_RootElementName()
        {
            var value = new XmlSource<Setting>()
            {
                FileName = @"TestFiles\XmlConfigs\XmlConfig_set_RootElementName.xml",
                RootElementName = "testConfig"
            }
             .Select("Setting1");
            Assert.AreEqual("Value1", value);
        }

        [TestMethod]
        public void set_SettingElementName()
        {
            var value = new XmlSource<Setting>()
            {
                FileName = @"TestFiles\XmlConfigs\XmlConfig_set_SettingElementName.xml",
                RootElementName = "testConfig",
                SettingElementName = "testSetting"
            }
             .Select("Setting1");
            Assert.AreEqual("Value1", value);
        }
        #endregion

    }
}
