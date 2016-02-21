using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.UnitTesting;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.Attributes.Constraints.RangeAttributeTests
{
    [TestClass]
    public class ValidateTests
    {
        [TestMethod]
        public void ValidatesDateTimeInRange()
        {
            ExceptionAssert.DoesNotThrow(() =>
            {
                var attr = new RangeAttribute(typeof(DateTime), "2015-06-15", "2015-07-15");
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
               var attr = new RangeAttribute(typeof(DateTime), "2015-06-15", "2015-07-15");
               attr.Validate(new DateTime(2015, 8, 1));
               Assert.IsTrue(true);
           }, ex => { },
            Assert.Fail);
        }
    }
}
