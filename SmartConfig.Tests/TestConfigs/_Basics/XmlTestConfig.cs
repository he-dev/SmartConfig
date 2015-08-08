using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class XmlTestConfig
    {
        public static XDocument XDocumentField;
        public static XElement XElementField;
    }
}
