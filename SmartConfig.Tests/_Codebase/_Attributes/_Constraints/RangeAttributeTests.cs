using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class RangeAttributeTests
    {
        [TestMethod]
        public void CreatesRangeAttribute()
        {
            var attr = new RangeAttribute { Type = typeof(int), Min = "1", Max = "3" };
            Assert.AreEqual(attr.Type, typeof(int));
            Assert.AreEqual("1", attr.Min);
            Assert.AreEqual("3", attr.Max);
        }

        [TestMethod]
        public void ValidatesDateTimeInRange()
        {
            ExceptionAssert.DoesNotThrow(() =>
            {
                var attr = new RangeAttribute { Type = typeof(DateTime), Min = "2015-06-15", Max = "2015-07-15" };
                attr.Validate(new DateTime(2015, 7, 1));
                Assert.IsTrue(true);
            },
            Assert.Fail);
        }

        [TestMethod]
        public void ThrowsDateTimeNotInRange()
        {
            ExceptionAssert.Throws<RangeViolationException>(() =>
           {
               var attr = new RangeAttribute { Type = typeof(DateTime), Min = "2015-06-15", Max = "2015-07-15" };
               attr.Validate(new DateTime(2015, 8, 1));
               Assert.IsTrue(true);
           }, ex => { },
            Assert.Fail);
        }
    }
}
