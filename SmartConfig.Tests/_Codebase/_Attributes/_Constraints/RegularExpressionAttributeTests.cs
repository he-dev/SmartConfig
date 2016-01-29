using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class RegularExpressionAttributeTests
    {
        [TestMethod]
        public void CreatesRegularExpressionAttribute()
        {
            var attr1 = new RegularExpressionAttribute { Pattern = "\\d" };
            Assert.AreEqual("\\d", attr1.Pattern);
        }

        [TestMethod]
        public void ValidatesString()
        {
            ExceptionAssert.DoesNotThrow(() =>
            {
                var attr1 = new RegularExpressionAttribute { Pattern = "\\d[A-Z]" };
                attr1.Validate("1B");
                Assert.IsTrue(true);
            }, Assert.Fail);
        }

        [TestMethod]
        public void ThrowsRegularExpressionViolationException()
        {
            ExceptionAssert.Throws<RegularExpressionViolationException>(() =>
            {
                var attr1 = new RegularExpressionAttribute { Pattern = "\\d[A-Z]" };
                attr1.Validate("1e");
            }, ex => { }, Assert.Fail);
        }
    }
}