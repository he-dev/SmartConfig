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
        public void DeserializeObject_XDocument()
        {
            const string xDocument = @"<?xml version=""1.0""?><testXml></testXml>";

            var converter = new XmlConverter();
            Assert.AreEqual(XDocument.Parse(xDocument).ToString(), converter.DeserializeObject(xDocument, typeof(XDocument), Enumerable.Empty<ConstraintAttribute>()).ToString());
        }

        [TestMethod]
        public void DeserializeObject_XElement()
        {
            const string xElement = @"<testXml></testXml>";

            var converter = new XmlConverter();
            Assert.AreEqual(XElement.Parse(xElement).ToString(), converter.DeserializeObject(xElement, typeof(XElement), Enumerable.Empty<ConstraintAttribute>()).ToString());
        }

        [TestMethod]
        public void TestSerializeObject()
        {
        }
    }
}
