using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;
using SmartUtilities.UnitTesting;

namespace SmartConfig.Tests.Converters.BooleanConverterTests
{
    [TestClass]
    public class DeserializeTests
    {
        [TestMethod]
        public void DeserializesBoolean()
        {
            var converter = new BooleanConverter();
            Assert.AreEqual(true, converter.DeserializeObject("true", typeof(bool), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(false, converter.DeserializeObject("false", typeof(bool), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void ThrowsDeserializationException()
        {
            var converter = new BooleanConverter();
            ExceptionAssert.Throws<DeserializationException>(() =>
            {
                converter.DeserializeObject("abc", typeof(bool), Enumerable.Empty<ConstraintAttribute>());
            }, ex => { }, Assert.Fail);
        }
    }

    [TestClass]
    public class SerializeTests
    {
        [TestMethod]
        public void SerializesBoolean()
        {
            var converter = new BooleanConverter();
            Assert.AreEqual(true, converter.SerializeObject(true, typeof(bool), Enumerable.Empty<Attribute>()));
            Assert.AreEqual(false, converter.SerializeObject(false, typeof(bool), Enumerable.Empty<Attribute>()));
        }

        [TestMethod]
        public void ThrowsSerializationException()
        {
            ExceptionAssert.Throws<SerializationException>(() =>
            {
                var converter = new BooleanConverter();
                converter.SerializeObject("abc", typeof(bool), Enumerable.Empty<ConstraintAttribute>());
            },
            ex => { },
            Assert.Fail);
        }
    }
}
