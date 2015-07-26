using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;

namespace SmartConfig.Tests.Converters
{
    [TestClass]
    public class JsonConverterTests
    {
        [TestMethod]
        public void TestDeserializeObject()
        {
            var converter = new JsonConverter();
            CollectionAssert.AreEqual(new Int32[] { 1, 2, 3 }, (List<Int32>)converter.DeserializeObject("[1, 2, 3]", typeof(List<Int32>), Enumerable.Empty<ValueContraintAttribute>()));
        }

        [TestMethod]
        public void TestSerializeObject()
        {
            var converter = new JsonConverter();
            Assert.AreEqual("[1,2,3]", converter.SerializeObject(new Int32[] { 1, 2, 3 }, null, Enumerable.Empty<ValueContraintAttribute>()));
        }
    }
}
