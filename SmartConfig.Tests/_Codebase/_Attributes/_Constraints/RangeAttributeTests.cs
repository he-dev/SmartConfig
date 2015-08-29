using SmartConfig;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class RangeAttributeTests
    {
        [TestMethod]
        public void ctor_RangeAttributeTest()
        {
            var attr = new RangeAttribute(typeof(int), "1", "3");
            Assert.AreEqual("1", attr.Min);
            Assert.AreEqual("3", attr.Max);
        }

        [TestMethod]
        public void IsValid_DateTime()
        {
            Assert.IsTrue(new RangeAttribute(typeof(DateTime), "2015-06-15", "2015-07-15").IsValid(new DateTime(2015, 7, 1)));
        }

        [TestMethod]
        public void IsValid_Int32()
        {
            Assert.IsTrue(new RangeAttribute(typeof(int), "0", "2").IsValid(1));
        }
    }
}
