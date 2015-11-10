using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class RegularExpressionAttributeTests
    {
        [TestMethod]
        public void ctor_RegularExpressionAttribute()
        {
            var attr1 = new RegularExpressionAttribute("\\d[A-Z]");
            Assert.AreEqual("\\d[A-Z]", attr1.ToString());
        }

        [TestMethod]
        public void IsMatch()
        {
            var attr1 = new RegularExpressionAttribute("\\d[A-Z]");
            Assert.IsFalse(attr1.IgnoreCase);
            Assert.IsTrue(attr1.IsMatch("1B"));
            Assert.IsFalse(attr1.IsMatch("1e"));

            var attr2 = new RegularExpressionAttribute("\\d[A-Z]", true);
            Assert.IsTrue(attr2.IgnoreCase);
            Assert.IsTrue(attr2.IsMatch("2c"));
            Assert.IsFalse(attr2.IsMatch("23"));
        }
    }
}