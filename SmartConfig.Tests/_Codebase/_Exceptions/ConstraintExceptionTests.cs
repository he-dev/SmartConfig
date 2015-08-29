using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass()]
    public class ConstraintExceptionTests
    {
        [TestMethod()]
        public void ctor_ConstriantException_DateTime()
        {
            var ex = new ConstraintException(new DateTimeFormatAttribute("ddMMyyyy"), "abc");
            Assert.AreEqual("abc", ex.Value);
            Assert.IsTrue(ex.Message.Contains("DateTimeFormat"));
            Assert.IsTrue(ex.Message.Contains("ddMMyyyy"));
        }

        [TestMethod()]
        public void ctor_ConstriantException_Range()
        {
            var ex = new ConstraintException(new RangeAttribute(typeof(int), "1", "3"), "4");
            Assert.AreEqual("4", ex.Value);
            Assert.IsTrue(ex.Message.Contains("Range"));
            Assert.IsTrue(ex.Message.Contains("Int32"));
            Assert.IsTrue(ex.Message.Contains("1"));
            Assert.IsTrue(ex.Message.Contains("3"));
        }

        [TestMethod()]
        public void ctor_ConstriantException_RegularExpression()
        {
            var ex = new ConstraintException(new RegularExpressionAttribute("\\d[A-Z]"), "44");
            Assert.AreEqual("44", ex.Value);
            Assert.IsTrue(ex.Message.Contains("RegularExpression"));
            Assert.IsTrue(ex.Message.Contains("\\d[A-Z]"));
        }
    }
}