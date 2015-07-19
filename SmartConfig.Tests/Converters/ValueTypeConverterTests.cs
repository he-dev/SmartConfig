using System;
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
            Assert.AreEqual(123, valueTypeConverter.DeserializeObject("123", typeof(Int32)));
            Assert.AreEqual(123, valueTypeConverter.DeserializeObject("123", typeof(Int32?)));
            Assert.IsNull(valueTypeConverter.DeserializeObject(null, typeof(Int32?)));
        }

        [TestMethod]
        public void TestSerializeObject()
        {
            var valueTypeConverter = new ValueTypeConverter();
            Assert.AreEqual("123", valueTypeConverter.SerializeObject(123));
            Assert.AreEqual("123", valueTypeConverter.SerializeObject((Int32?)123));
            Assert.IsNull(valueTypeConverter.SerializeObject((Int32?)null));
        }
    }
}
