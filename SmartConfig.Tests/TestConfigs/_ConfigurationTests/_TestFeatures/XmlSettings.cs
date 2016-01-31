using System.Xml.Linq;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class XmlSettings
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        public static XDocument XDocumentSetting { get; set; }
        public static XElement XElementSetting { get; set; }
    }
}
