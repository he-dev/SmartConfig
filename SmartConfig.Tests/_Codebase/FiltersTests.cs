using SmartConfig;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;

namespace SmartConfig.Tests
{
    [TestClass]
    public class FiltersTests
    {
        [TestMethod]
        public void FilterByString()
        {
            var settings = new[]
            {
                new TestSetting("a|1.0.0|name|value"),
                new TestSetting("b|1.0.0|name|value"),
                new TestSetting("*|1.0.0|name|value"),
            };

            IEnumerable<IIndexer> result;
            IIndexer setting;

            result = Filters.FilterByString(settings, new KeyValuePair<string, string>("Environment", "a"));
            setting = result.FirstOrDefault();
            Assert.IsNotNull(setting);
            Assert.AreEqual(settings[0], setting);

            result = Filters.FilterByString(settings, new KeyValuePair<string, string>("Environment", "b"));
            setting = result.FirstOrDefault();
            Assert.IsNotNull(setting);
            Assert.AreEqual(settings[1], setting);

            result = Filters.FilterByString(settings, new KeyValuePair<string, string>("Environment", "c"));
            setting = result.FirstOrDefault();
            Assert.IsNotNull(setting);
            Assert.AreEqual(settings[2], setting);
        }

        [TestMethod]
        public void FilterByVersion()
        {
           var settings = new[]
           {
                new TestSetting("a|1.0.5|name|value"),
                new TestSetting("a|*|name|value"),
                new TestSetting("a|1.1.0|name|value"),
                new TestSetting("a|2.0.5|name|value"),
            };

            IEnumerable<IIndexer> result;
            IIndexer setting;

            result = Filters.FilterByVersion(settings, new KeyValuePair<string, string>("Version", "1.0.5"));
            setting = result.FirstOrDefault();
            Assert.AreEqual(settings[0], setting);

            result = Filters.FilterByVersion(settings, new KeyValuePair<string, string>("Version", "1.2.0"));
            setting = result.FirstOrDefault();
            Assert.AreEqual(settings[2], setting);

            result = Filters.FilterByVersion(settings, new KeyValuePair<string, string>("Version", "1.0.0"));
            setting = result.FirstOrDefault();
            Assert.AreEqual(settings[1], setting);
        }

        [TestMethod]
        public void FilterByString_Then_FilterByVersion()
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
        }
    }
}
