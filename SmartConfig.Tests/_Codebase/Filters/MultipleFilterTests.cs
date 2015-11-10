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
                new TestSetting("a|*|name|value"),
                new TestSetting("a|1.0.0|name|value"),
                new TestSetting("b|1.0.0|name|value"),
                new TestSetting("b|1.0.5|name|value"),
                new TestSetting("b|*|name|value"),
                new TestSetting("*|1.0.0|name|value"),
            };

            IEnumerable<IIndexer> result;
            IIndexer setting;

            result = new StringFilter().FilterSettings(settings, new SettingKey("Environment", "b"));
            result = new VersionFilter().FilterSettings(result, new SettingKey("Version", "1.0.1"));
            setting = result.FirstOrDefault();
            Assert.AreEqual(settings[2], setting);

            result = new VersionFilter().FilterSettings(settings, new SettingKey("Version", "1.0.1"));
            result = new StringFilter().FilterSettings(result, new SettingKey("Environment", "b"));
            setting = result.FirstOrDefault();
            Assert.AreEqual(settings[2], setting);
        }
    }
}
