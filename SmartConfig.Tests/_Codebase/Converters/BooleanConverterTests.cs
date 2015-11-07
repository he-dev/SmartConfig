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
        public void DeserializeObject_Int32()
        {
            var valueTypeConverter = new ValueTypeConverter();
            Assert.AreEqual(123, valueTypeConverter.DeserializeObject("123", typeof(Int32), Enumerable.Empty<ConstraintAttribute>()));
            //Assert.AreEqual(123, valueTypeConverter.DeserializeObject("123", typeof(Int32?), Enumerable.Empty<ConstraintAttribute>()));
            //Assert.IsNull(valueTypeConverter.DeserializeObject(null, typeof(Int32?), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void SerializeObject_Int32()
        {
            var valueTypeConverter = new ValueTypeConverter();
            Assert.AreEqual("123", valueTypeConverter.SerializeObject(123, typeof(int), Enumerable.Empty<ConstraintAttribute>()));
            //Assert.AreEqual("123", valueTypeConverter.SerializeObject((Int32?)123, typeof(int), Enumerable.Empty<ConstraintAttribute>()));
            //Assert.IsNull(valueTypeConverter.SerializeObject(null, typeof(int?), Enumerable.Empty<ConstraintAttribute>()));
        }
    }
}
