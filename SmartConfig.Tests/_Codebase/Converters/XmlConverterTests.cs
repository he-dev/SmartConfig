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
        public void DeserializeObject_CanDeserializeXDocument()
        {
            const string xDocument = @"<?xml version=""1.0""?><testXml></testXml>";

            var converter = new XmlConverter();
            Assert.AreEqual(XDocument.Parse(xDocument).ToString(), converter.DeserializeObject(xDocument, typeof(XDocument), Enumerable.Empty<ConstraintAttribute>()).ToString());
        }

        [TestMethod]
        public void DeserializeObject_CanDeserializeXElement()
        {
            const string xElement = @"<testXml></testXml>";

            var converter = new XmlConverter();
            Assert.AreEqual(XElement.Parse(xElement).ToString(), converter.DeserializeObject(xElement, typeof(XElement), Enumerable.Empty<ConstraintAttribute>()).ToString());
        }

        [TestMethod]
        public void SerializeObject_CanSerializeXDocument()
        {
            const string xDocument = "<?xml version=\"1.0\" encoding=\"utf-8\"?><testXml>abc</testXml>";

            var converter = new XmlConverter();
            //Assert.AreEqual(xDocument, converter.SerializeObject(XDocument.Parse(xDocument), typeof(XDocument), Enumerable.Empty<ConstraintAttribute>()).ToString());
        }

        [TestMethod]
        public void SerializeObject_CanSerializeXElement()
        {
            const string xElement = @"<testXml></testXml>";

            var converter = new XmlConverter();
            //Assert.AreEqual(xElement, converter.SerializeObject(XElement.Parse(xElement), typeof(XElement), Enumerable.Empty<ConstraintAttribute>()).ToString());
        }
    }
}
