using System;
using System.Collections.Generic;
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
            Assert.AreEqual(XDocument.Parse(xDocument).ToString(), converter.DeserializeObject(xDocument, typeof(XDocument)).ToString());
            Assert.AreEqual(XElement.Parse(xElement).ToString(), converter.DeserializeObject(xElement, typeof(XElement)).ToString());
        }

        [TestMethod]
        public void TestSerializeObject()
        {
        }
    }
}
