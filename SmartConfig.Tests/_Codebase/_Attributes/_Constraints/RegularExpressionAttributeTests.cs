using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class RegularExpressionAttributeTests
    {
        [TestMethod]
        [ExpectedException(typeof(PropertyNotSetException))]
        public void ctor_RegularExpressionAttribute()
        {
            var attr1 = new RegularExpressionAttribute();
            attr1.Validate("abc");
        }

        [TestMethod]
        public void Validate_CanValidateString()
        {
            var attr1 = new RegularExpressionAttribute { Pattern = "\\d[A-Z]" };
            attr1.Validate("1B");
        }

        [TestMethod]
        [ExpectedException(typeof(RegularExpressionViolationException))]
        public void Validate_ThrowsRegularExpressionViolationException()
        {
            var attr1 = new RegularExpressionAttribute { Pattern = "\\d[A-Z]" };
            attr1.Validate("1e");
        }
    }
}