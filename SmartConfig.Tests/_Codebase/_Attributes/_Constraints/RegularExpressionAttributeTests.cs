using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartUtilities.UnitTesting;

// ReSharper disable once CheckNamespace
namespace SmartConfig.Tests.Attributes.Constraints.RegularExpressionAttributeTests
{
    [TestClass]
    public class ValidateTests
    {
        [TestMethod]
        public void ValidatesString()
        {
            ExceptionAssert.DoesNotThrow(() =>
            {
                var attr1 = new RegularExpressionAttribute("\\d[A-Z]");
                attr1.Validate("1B");
                Assert.IsTrue(true);
            }, Assert.Fail);
        }

        [TestMethod]
        public void ThrowsRegularExpressionViolationException()
        {
            ExceptionAssert.Throws<RegularExpressionViolationException>(() =>
            {
                var attr1 = new RegularExpressionAttribute("\\d[A-Z]");
                attr1.Validate("1e");
            }, ex => { }, Assert.Fail);
        }
    }
}