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
            var attr = new RangeAttribute { Type = typeof(int), Min = "1", Max = "3" };
            Assert.AreEqual("1", attr.Min);
            Assert.AreEqual("3", attr.Max);
        }

        [TestMethod]
        public void IsValid_DateTime()
        {
            var attr = new RangeAttribute { Type = typeof(DateTime), Min = "2015-06-15", Max = "2015-07-15" };
            attr.Validate(new DateTime(2015, 7, 1));
        }

        [TestMethod]
        public void IsValid_Int32()
        {
            var attr = new RangeAttribute { Type = typeof(int), Min = "0", Max = "2" };
            attr.Validate(1);
        }
    }
}
