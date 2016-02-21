using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartConfig.Converters;

namespace SmartConfig.Core.Tests.Converters.XmlConverterTests
{
    [TestClass]
    public class DeserializeTests
    {
        [TestMethod]
        public void DeserializesXDocument()
        {
            const string xDocument = @"<?xml version=""1.0""?><testXml></testXml>";

            var converter = new XmlConverter();
            Assert.AreEqual(
                XDocument.Parse(xDocument).ToString(),
                converter.DeserializeObject(
                    xDocument,
                    typeof(XDocument),
                    Enumerable.Empty<ConstraintAttribute>()
                ).ToString());
        }

        [TestMethod]
        public void DeserializesXElement()
        {
            const string xElement = @"<testXml></testXml>";

            var converter = new XmlConverter();
            Assert.AreEqual(
                XElement.Parse(xElement).ToString(),
                converter.DeserializeObject(
                    xElement, 
                    typeof(XElement), 
                    Enumerable.Empty<ConstraintAttribute>()
                ).ToString());
        }

    }

    [TestClass]
    public class SerializeTests
    {
        [TestMethod]
        public void SerializesXDocumentl()
        {
            var xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><foo>bar</foo>";

            var converter = new XmlConverter();
            Assert.AreEqual(
                xml, 
                converter.SerializeObject(
                    XDocument.Parse(xml), 
                    typeof(string), 
                    Enumerable.Empty<ConstraintAttribute>()
                ).ToString());
        }

        [TestMethod]
        public void SerializesXElement()
        {
            var xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><foo>bar</foo>";

            var converter = new XmlConverter();
            Assert.AreEqual(
                xml, 
                converter.SerializeObject(
                    XElement.Parse(xml), 
                    typeof(string), 
                    Enumerable.Empty<ConstraintAttribute>()
                ).ToString());
        }
    }
}
