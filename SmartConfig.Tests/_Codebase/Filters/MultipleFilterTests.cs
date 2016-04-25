using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Data;
using SmartConfig.Filters;

// ReSharper disable InconsistentNaming

namespace SmartConfig.Core.Tests.Filters.MultipleFilterTests
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

            result = new StringFilter().Apply(settings, new KeyValuePair<string, object>("Environment", "b"));
            result = new VersionFilter().Apply(result, new KeyValuePair<string, object>("Version", "1.0.1"));
            setting = result.FirstOrDefault();
            Assert.AreEqual(settings[2], setting);

            result = new VersionFilter().Apply(settings, new KeyValuePair<string, object>("Version", "1.0.1"));
            result = new StringFilter().Apply(result, new KeyValuePair<string, object>("Environment", "b"));
            setting = result.FirstOrDefault();
            Assert.AreEqual(settings[2], setting);
        }
    }
}
