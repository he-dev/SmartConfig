using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;
using SmartConfig.Reflection;
using SmartConfig.Tests.TestConfigs;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests.Reflection
{
    [TestClass]
    public class ConfigurationInfo_ctor
    {
        [SmartConfig]
        private static class TestConfig1
        {
            public static string StringSetting { get; set; }
            public static int Int32Setting { get; set; }
        }

        [TestMethod]
        public void param_configType_MustNotBeNull()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                Configuration.LoadSettings(null);
            }, ex =>
            {
                Assert.AreEqual("configType", ex.ParamName);
            },
            Assert.Fail);
        }

        [TestMethod]
        public void param_configType_MustHaveSmartConfigAttribute()
        {
            ExceptionAssert.Throws<SmartConfigAttributeMissingException>(() =>
            {
                Configuration.LoadSettings(typeof(ConfigTypeMustBeMarkedWithSmartConfigAttribute));
            }, ex =>
            {
                Assert.AreEqual(typeof(ConfigTypeMustBeMarkedWithSmartConfigAttribute).FullName, ex.ConfigTypeFullName);
            },
            Assert.Fail);
        }

        [TestMethod]
        public void param_configType_MustBeStatic()
        {
            ExceptionAssert.Throws<ConfigTypeNotStaticException>(() =>
            {
                Configuration.LoadSettings(typeof(ConfigTypeMustBeStatic));
            }, ex =>
            {
                Assert.AreEqual(typeof(ConfigTypeMustBeStatic).FullName, ex.ConfigTypeFullName);
            },
            Assert.Fail);
        }

        [TestMethod]
        public void ctor_CanExamineConfigType()
        {
            var configInfo = new ConfigurationInfo(typeof(TestConfig1));

            Assert.IsNotNull(configInfo);
            Assert.AreEqual(2, configInfo.SettingInfos.Count);
        }
    }
}