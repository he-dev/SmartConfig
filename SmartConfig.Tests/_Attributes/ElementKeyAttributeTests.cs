using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class ElementKeyAttributeTests
    {
        [TestMethod]
        public void ElementKeyAttribute_String()
        {
            var elementKey = new FieldKeyAttribute("Name=Test");
            Assert.AreEqual("Name", elementKey.Key);
            Assert.AreEqual("Test", elementKey.Value);
        }
    }
}
