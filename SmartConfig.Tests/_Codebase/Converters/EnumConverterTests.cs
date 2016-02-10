using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;

namespace SmartConfig.Tests.Converters.EnumConverterTests
{
    [TestClass]
    public class DeserializeTests
    {
        [TestMethod]
        public void DeserializesEnum()
        {
            var converter = new EnumConverter();
            Assert.AreEqual(TestEnum.TestValue2,
                converter.DeserializeObject("TestValue2", typeof (TestEnum), Enumerable.Empty<ConstraintAttribute>()));
        }

    }
    [TestClass]
    public class SerializeTests
    {
        [TestMethod]
        public void SerializesEnum()
        {
            var converter = new EnumConverter();
            Assert.AreEqual("TestValue2", converter.SerializeObject(TestEnum.TestValue2, typeof(string), Enumerable.Empty<ConstraintAttribute>()));
        }
    }
}
