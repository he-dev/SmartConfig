using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.Filters;

// ReSharper disable InconsistentNaming

namespace SmartConfig.Tests.Filters
{
    [TestClass]
    public class StringFilter_Filter_Method
    {
        [TestMethod]
        public void CanFindSingleValue()
        {
            var testSettings = new[]
            {
                new CustomTestSetting("a|1.0.0|name|value"),
                new CustomTestSetting("b|1.0.0|name|value"),
                new CustomTestSetting("d|1.0.0|name|value"),
            };

            var result = new StringKeyFilter().Apply(testSettings, new SettingKey("Environment", "b"));
            var setting = result.FirstOrDefault();
            Assert.IsNotNull(setting);
            Assert.AreEqual(testSettings[1], setting);
        }

        [TestMethod]
        public void CanFindDefualtValue()
        {
            var settings = new[]
            {
                new CustomTestSetting("a|1.0.0|name|value"),
                new CustomTestSetting("b|1.0.0|name|value"),
                new CustomTestSetting("*|1.0.0|name|value"),
                new CustomTestSetting("d|1.0.0|name|value"),
            };

            var result = new StringKeyFilter().Apply(settings, new SettingKey("Environment", "c"));
            var setting = result.FirstOrDefault();
            Assert.IsNotNull(setting);
            Assert.AreEqual(settings[2], setting);
        }

        [TestMethod]
        public void CanFindNothing()
        {
            var settings = new[]
            {
                new CustomTestSetting("a|1.0.0|name|value"),
                new CustomTestSetting("b|1.0.0|name|value"),
                new CustomTestSetting("d|1.0.0|name|value"),
            };

            var result = new StringKeyFilter().Apply(settings, new SettingKey("Environment", "c"));
            var setting = result.FirstOrDefault();
            Assert.IsNull(setting);
        }
    }
}
