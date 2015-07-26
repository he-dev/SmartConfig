using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;

namespace SmartConfig.Tests.Converters
{
    [TestClass]
    public class ValueTypeConverterTests
    {
        [TestMethod]
        public void TestDeserializeObject()
        {
            var valueTypeConverter = new ValueTypeConverter();
            Assert.AreEqual(123, valueTypeConverter.DeserializeObject("123", typeof(Int32), Enumerable.Empty<ValueContraintAttribute>()));
            Assert.AreEqual(123, valueTypeConverter.DeserializeObject("123", typeof(Int32?), Enumerable.Empty<ValueContraintAttribute>()));
            Assert.IsNull(valueTypeConverter.DeserializeObject(null, typeof(Int32?), Enumerable.Empty<ValueContraintAttribute>()));
        }

        [TestMethod]
        public void TestSerializeObject()
        {
            var valueTypeConverter = new ValueTypeConverter();
            Assert.AreEqual("123", valueTypeConverter.SerializeObject(123, typeof(int), Enumerable.Empty<ValueContraintAttribute>()));
            Assert.AreEqual("123", valueTypeConverter.SerializeObject((Int32?)123, typeof(int), Enumerable.Empty<ValueContraintAttribute>()));
            Assert.IsNull(valueTypeConverter.SerializeObject((Int32?)null, typeof(int?), Enumerable.Empty<ValueContraintAttribute>()));
        }
    }
}
