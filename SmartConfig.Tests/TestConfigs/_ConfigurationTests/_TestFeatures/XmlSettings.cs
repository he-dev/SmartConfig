using System.Xml.Linq;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class XmlSettings
    {
        public static XDocument XDocumentSetting { get; set; }
        public static XElement XElementSetting { get; set; }
    }
}
