using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests.Converters
{
    [TestClass]
    public class BooleanConverterTests
    {
        [TestMethod]
        public void DeserializeObject_CanDeserializeBoolean()
        {
            var converter = new BooleanConverter();
            Assert.AreEqual(true, converter.DeserializeObject("true", typeof(bool), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(false, converter.DeserializeObject("false", typeof(bool), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void DeserializeObject_ThrowsExceptionForInvalidNonBoolean()
        {
            var converter = new BooleanConverter();
            ExceptionAssert.Throws<TargetInvocationException>(() =>
            {
                converter.DeserializeObject("abc", typeof(bool), Enumerable.Empty<ConstraintAttribute>());
            }, ex => { }, Assert.Fail);
        }

        [TestMethod]
        public void SerializeObject_CanSerializeBoolean()
        {
            var converter = new BooleanConverter();
            Assert.AreEqual(true, converter.SerializeObject(true, typeof(bool), Enumerable.Empty<Attribute>()));
            Assert.AreEqual(false, converter.SerializeObject(false, typeof(bool), Enumerable.Empty<Attribute>()));
        }

        [TestMethod]
        public void SerializeObject_ThrowsExceptionForInvalidBoolean()
        {
            var converter = new BooleanConverter();
            ExceptionAssert.Throws<TargetException>(() =>
            {
                converter.SerializeObject("abc", typeof(bool), Enumerable.Empty<ConstraintAttribute>());
            }, ex => { }, Assert.Fail);
        }
    }
}
