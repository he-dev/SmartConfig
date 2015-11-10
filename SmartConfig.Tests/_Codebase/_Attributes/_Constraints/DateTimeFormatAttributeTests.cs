using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class DateTimeFormatAttributeTests
    {
        [TestMethod]
        public void ctor_CanCreateDateTimeFormatAttribute()
        {
            const string format = "ddMMyyyy";
            var attr = new DateTimeFormatAttribute(format);
            Assert.AreEqual(format, attr.Format);
            Assert.AreEqual(format, attr.ToString());
        }

        [TestMethod]
        public void TryParseExact_CanParseDateTime()
        {
            const string format = "ddMMyyyy";
            var attr = new DateTimeFormatAttribute(format);
            DateTime result;
            Assert.IsTrue(attr.TryParseExact("12092015", out result));
        }

        [TestMethod]
        public void TryParseExact_FailsToParseInvalidDateTime()
        {
            const string format = "ddMMMyyyy";
            var attr = new DateTimeFormatAttribute(format);
            DateTime result;
            Assert.IsFalse(attr.TryParseExact("12092015", out result));
        }
    }
}