using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Core.Tests.Converters
{
    [TestClass]
    public class StringConverterTests
    {
        [TestMethod]
        public void DeserializeObject_CanDeserializeString()
        {
            var converter = new StringConverter();
            Assert.AreEqual("abc", converter.DeserializeObject("abc", typeof(string), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void DeserializeObject_CanValidateStringWithRegex()
        {
            var converter = new StringConverter();
            Assert.AreEqual("21", converter.DeserializeObject("21", typeof(string), new[] { new RegularExpressionAttribute(@"\d{2}") }));
        }

        [TestMethod]
        public void DeserializeObject_ThrowsExceptionWhenStringDoesnotMatchRegex()
        {
            var converter = new StringConverter();
            ExceptionAssert.Throws<RegularExpressionViolationException>(() =>
            {
                converter.DeserializeObject("7", typeof(string), new[] { new RegularExpressionAttribute(@"\d{2}") });
            }, ex => { }, Assert.Fail);
        }

        [TestMethod]
        public void SerializeObject_CanSerializeString()
        {
            var converter = new StringConverter();
            Assert.AreEqual("abc", converter.SerializeObject("abc", typeof(string), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void SerializeObject_ThrowsExceptionWhenStringDoesnotMatchRegex()
        {
            var converter = new StringConverter();
            ExceptionAssert.Throws<RegularExpressionViolationException>(() =>
            {
                converter.SerializeObject("7", typeof(string), new[] { new RegularExpressionAttribute(@"\d{2}") });
            }, ex => { }, Assert.Fail);
        }

    }
}
