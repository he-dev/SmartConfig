using SmartConfig;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
// ReSharper disable InconsistentNaming

namespace SmartConfig.Tests
{
    [TestClass]
    public class FiltersTests
    {
        [TestClass]
        public class FilterByString_TestFeatures
        {
            [TestMethod]
            public void FilterByString_CanFindSingleValue()
            {
                var testSettings = new[]
                {
                    new TestSetting("a|1.0.0|name|value"),
                    new TestSetting("b|1.0.0|name|value"),
                    new TestSetting("d|1.0.0|name|value"),
                };

                var result = Filters.FilterByString(testSettings, new KeyValuePair<string, string>("Environment", "b"));
                var setting = result.FirstOrDefault();
                Assert.IsNotNull(setting);
                Assert.AreEqual(testSettings[1], setting);
            }

            [TestMethod]
            public void FilterByString_CanFindDefualtValue()
            {
                var settings = new[]
                {
                    new TestSetting("a|1.0.0|name|value"),
                    new TestSetting("b|1.0.0|name|value"),
                    new TestSetting("*|1.0.0|name|value"),
                    new TestSetting("d|1.0.0|name|value"),
                };

                var result = Filters.FilterByString(settings, new KeyValuePair<string, string>("Environment", "c"));
                var setting = result.FirstOrDefault();
                Assert.IsNotNull(setting);
                Assert.AreEqual(settings[2], setting);
            }

            [TestMethod]
            public void FilterByString_CanFindNothing()
            {
                var settings = new[]
                {
                    new TestSetting("a|1.0.0|name|value"),
                    new TestSetting("b|1.0.0|name|value"),
                    new TestSetting("d|1.0.0|name|value"),
                };

                var result = Filters.FilterByString(settings, new KeyValuePair<string, string>("Environment", "c"));
                var setting = result.FirstOrDefault();
                Assert.IsNull(setting);
            }
        }

        [TestClass]
        public class FilterByVersion_TestFeatures
        {
            [TestMethod]
            public void FilterByVersion_CanFindExactVersion()
            {
                var settings = new[]
                {
                    new TestSetting("a|1.0.5|name|value"),
                    new TestSetting("a|*|name|value"),
                    new TestSetting("a|1.1.0|name|value"),
                    new TestSetting("a|2.0.5|name|value"),
                };

                var result = Filters.FilterByVersion(settings, new KeyValuePair<string, string>("Version", "1.1.0"));
                var setting = result.FirstOrDefault();
                Assert.AreEqual(settings[2], setting);
            }

            [TestMethod]
            public void FilterByVersion_CanFindPreviousVersion()
            {
                var settings = new[]
                {
                    new TestSetting("a|1.0.4|name|value"),
                    new TestSetting("a|1.0.5|name|value"),
                    new TestSetting("a|*|name|value"),
                    new TestSetting("a|1.1.0|name|value"),
                    new TestSetting("a|2.0.5|name|value"),
                };

                var result = Filters.FilterByVersion(settings, new KeyValuePair<string, string>("Version", "1.0.7"));
                var setting = result.FirstOrDefault();
                Assert.AreEqual(settings[1], setting);
            }

            [TestMethod]
            public void FilterByVersion_CanFindDefaultVersion()
            {
                var settings = new[]
                {
                    new TestSetting("a|1.0.5|name|value"),
                    new TestSetting("a|*|name|value"),
                    new TestSetting("a|1.1.0|name|value"),
                    new TestSetting("a|2.0.5|name|value"),
                };

                var result = Filters.FilterByVersion(settings, new KeyValuePair<string, string>("Version", "1.0.3"));
                var setting = result.FirstOrDefault();
                Assert.AreEqual(settings[1], setting);
            }

            [TestMethod]
            public void FilterByVersion_CanFindNothing()
            {
                var settings = new[]
                {
                    new TestSetting("a|1.0.5|name|value"),
                    new TestSetting("a|1.1.0|name|value"),
                    new TestSetting("a|2.0.5|name|value"),
                };

                var result = Filters.FilterByVersion(settings, new KeyValuePair<string, string>("Version", "1.0.2"));
                var setting = result.FirstOrDefault();
                Assert.IsNull(setting);
            }
        }

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

            result = Filters.FilterByString(settings, new KeyValuePair<string, string>("Environment", "b"));
            result = Filters.FilterByVersion(result, new KeyValuePair<string, string>("Version", "1.0.1"));
            setting = result.FirstOrDefault();
            Assert.AreEqual(settings[2], setting);

            result = Filters.FilterByVersion(settings, new KeyValuePair<string, string>("Version", "1.0.1"));
            result = Filters.FilterByString(result, new KeyValuePair<string, string>("Environment", "b"));
            setting = result.FirstOrDefault();
            Assert.AreEqual(settings[2], setting);
        }
    }
}
