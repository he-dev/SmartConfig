using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;

namespace SmartConfig.Tests.Converters
{
    [TestClass]
    public class BooleanConverterTests
    {
        [TestMethod]
        public void DeserializeObject_Boolean()
        {
            var converter = new BooleanConverter();
            Assert.AreEqual(true, converter.DeserializeObject("true", typeof(bool), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual(false, converter.DeserializeObject("false", typeof(bool), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void SerializeObject_Boolean()
        {
            var converter = new BooleanConverter();
            Assert.AreEqual("True", converter.SerializeObject(true, typeof(bool), Enumerable.Empty<ConstraintAttribute>()));
            Assert.AreEqual("False", converter.SerializeObject(false, typeof(bool), Enumerable.Empty<ConstraintAttribute>()));
            //Assert.AreEqual("123", valueTypeConverter.SerializeObject((Int32?)123, typeof(int), Enumerable.Empty<ConstraintAttribute>()));
            //Assert.IsNull(valueTypeConverter.SerializeObject(null, typeof(int?), Enumerable.Empty<ConstraintAttribute>()));
        }
    }
}
