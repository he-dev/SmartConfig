using System.Xml.Linq;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class XmlTestConfig
    {
        public static XDocument XDocumentField { get; set; }
        public static XElement XElementField { get; set; }
    }
}
