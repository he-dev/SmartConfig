using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;

namespace SmartConfig.Tests.Converters
{
    [TestClass]
    public class XmlConverterTests
    {
        [TestMethod]
        public void TestDeserializeObject()
        {
            const string xDocument = @"<?xml version=""1.0""?><testXml></testXml>";
            const string xElement = @"<testXml></testXml>";

            var converter = new XmlConverter();
            Assert.AreEqual(XDocument.Parse(xDocument).ToString(), converter.DeserializeObject(xDocument, typeof(XDocument), Enumerable.Empty<ValueConstraintAttribute>()).ToString());
            Assert.AreEqual(XElement.Parse(xElement).ToString(), converter.DeserializeObject(xElement, typeof(XElement), Enumerable.Empty<ValueConstraintAttribute>()).ToString());
        }

        [TestMethod]
        public void TestSerializeObject()
        {
        }
    }
}
