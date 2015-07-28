using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class RangeAttributeTests
    {
        [TestMethod]
        public void Range_Long()
        {
            Assert.IsTrue(new RangeAttribute(typeof(DateTime), "2015-06-15", "2015-07-15").IsValid(new DateTime(2015, 7, 1)));
            Assert.IsTrue(new RangeAttribute(typeof(int), "0", "2").IsValid(1));
            //Assert.IsTrue(new RangeAttribute(null, 2).IsInRange(1));
            //Assert.IsTrue(new RangeAttribute(0, null).IsInRange(1));
            //Assert.IsFalse(new RangeAttribute(0, 2).IsInRange(3));
            //Assert.IsFalse(new RangeAttribute(null, 2).IsInRange(3));
            //Assert.IsFalse(new RangeAttribute(0, null).IsInRange(-1));
        }
    }
}
