using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;

namespace SmartConfig.Tests.Converters
{
    [TestClass]
    public class EnumConverterTests
    {
        [TestMethod]
        public void DeserializeObject_CanDeserializeEnum()
        {
            var converter = new EnumConverter();
            Assert.AreEqual(TestEnum.TestValue2, converter.DeserializeObject("TestValue2", typeof(TestEnum), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void SerializeObject_CanSerializeEnum()
        {
            var converter = new EnumConverter();
            Assert.AreEqual("TestValue2", converter.SerializeObject(TestEnum.TestValue2, typeof(string), Enumerable.Empty<ConstraintAttribute>()));
        }
    }
}
