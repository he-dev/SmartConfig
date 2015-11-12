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
        public void DeserializeObject_CanDeserializeListInt32()
        {
            var converter = new JsonConverter();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, (List<int>)converter.DeserializeObject("[1, 2, 3]", typeof(List<int>), Enumerable.Empty<ConstraintAttribute>()));
        }

        [TestMethod]
        public void SerializeObject_CanSerializeArrayInt32()
        {
            var converter = new JsonConverter();
            Assert.AreEqual("[1,2,3]", converter.SerializeObject(new[] { 1, 2, 3 }, null, Enumerable.Empty<ConstraintAttribute>()));
        }
    }
}
