using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Filters;

// ReSharper disable InconsistentNaming

namespace SmartConfig.Core.Tests.Filters.VersionFilterTests
{
    [TestClass]
    public class Filters_FilterByVersion_Method
    {
        [TestMethod]
        public void FilterByVersion_CanFindExactVersion()
        {
            var settings = new[]
            {
                new CustomTestSetting("a|1.0.5|name|value"),
                new CustomTestSetting("a|*|name|value"),
                new CustomTestSetting("a|1.1.0|name|value"),
                new CustomTestSetting("a|2.0.5|name|value"),
            };

            var result = new VersionFilter().Apply(settings, new KeyValuePair<string, object>("Version", "1.1.0"));
            var setting = result.FirstOrDefault();
            Assert.AreEqual(settings[2], setting);
        }

        [TestMethod]
        public void FilterByVersion_CanFindPreviousVersion()
        {
            var settings = new[]
            {
                new CustomTestSetting("a|1.0.5|name|value"),
                new CustomTestSetting("a|1.0.4|name|value"),
                new CustomTestSetting("a|*|name|value"),
                new CustomTestSetting("a|1.1.0|name|value"),
                new CustomTestSetting("a|2.0.5|name|value"),
            };

            var result = new VersionFilter().Apply(settings, new KeyValuePair<string, object>("Version", "1.0.7"));
            var setting = result.FirstOrDefault();
            Assert.AreEqual(settings[0], setting);
        }

        [TestMethod]
        public void FilterByVersion_CanFindDefaultVersion()
        {
            var settings = new[]
            {
                    new CustomTestSetting("a|1.0.5|name|value"),
                    new CustomTestSetting("a|*|name|value"),
                    new CustomTestSetting("a|1.1.0|name|value"),
                    new CustomTestSetting("a|2.0.5|name|value"),
                };

            var result = new VersionFilter().Apply(settings, new KeyValuePair<string, object>("Version", "1.0.3"));
            var setting = result.FirstOrDefault();
            Assert.AreEqual(settings[1], setting);
        }

        [TestMethod]
        public void FilterByVersion_CanFindNothing()
        {
            var settings = new[]
            {
                new CustomTestSetting("a|1.0.5|name|value"),
                new CustomTestSetting("a|1.1.0|name|value"),
                new CustomTestSetting("a|2.0.5|name|value"),
            };

            var result = new VersionFilter().Apply(settings, new KeyValuePair<string, object>("Version", "1.0.2"));
            var setting = result.FirstOrDefault();
            Assert.IsNull(setting);
        }
    }
}
