using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.Filters;

// ReSharper disable InconsistentNaming

namespace SmartConfig.Tests.Filters
{
   
    [TestClass]
    public class Filters_Mixed
    {
        [TestMethod]
        public void CanFilterByMultipleCriteria()
        {
            var settings = new[]
            {
                new CustomTestSetting("a|*|name|value"),
                new CustomTestSetting("a|1.0.0|name|value"),
                new CustomTestSetting("b|1.0.0|name|value"),
                new CustomTestSetting("b|1.0.5|name|value"),
                new CustomTestSetting("b|*|name|value"),
                new CustomTestSetting("*|1.0.0|name|value"),
            };

            IEnumerable<IIndexable> result;
            IIndexable setting;

            result = new StringKeyFilter().Apply(settings, new SettingKey("Environment", "b"));
            result = new VersionKeyFilter().Apply(result, new SettingKey("Version", "1.0.1"));
            setting = result.FirstOrDefault();
            Assert.AreEqual(settings[2], setting);

            result = new VersionKeyFilter().Apply(settings, new SettingKey("Version", "1.0.1"));
            result = new StringKeyFilter().Apply(result, new SettingKey("Environment", "b"));
            setting = result.FirstOrDefault();
            Assert.AreEqual(settings[2], setting);
        }
    }
}
