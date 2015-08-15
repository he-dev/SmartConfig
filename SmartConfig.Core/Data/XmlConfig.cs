using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using SmartUtilities;

namespace SmartConfig.Data
{
    public class XmlConfig<TConfigElement> : DataSource<TConfigElement> where TConfigElement : ConfigElement, new()
    {
        private XDocument xConfig;

        public XmlConfig()
        {
            Keys = new Dictionary<string, string>();
        }

        public string FileName { get; set; }       

        public override string Select(IDictionary<string, string> keys)
        {
            LoadXml();

            var allKeys = Utilities.CombineDictionaries(keys, Keys);

            var xElements = xConfig.Root.Elements()
                .Where(e => e.Attribute("name").Value.Equals(keys[KeyNames.DefaultKeyName], StringComparison.OrdinalIgnoreCase));

            // create TConfigElement from each item
            var elements = xElements.Select(x =>
            {
                var element = new TConfigElement
                {
                    Name = x.Attribute("name").Value,
                    Value = x.Value
                };

                // set custom properties
                foreach (var property in element.CustomProperties)
                {
                    var attr = x.Attributes().SingleOrDefault(a => a.Name.ToString().Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                    element.SetStringDelegates[property.Name](attr == null ? Wildcards.Asterisk : attr.Value);
                }
                return element;
            });

            elements = allKeys
                .Where(x => x.Key != KeyNames.DefaultKeyName)
                .Aggregate(elements, (current, keyValue) => Filters[keyValue.Key](current, keyValue));

            var result = elements.FirstOrDefault();
            return result != null ? result.Value : null;
        }

        public override void Update(IDictionary<string, string> keys, string value)
        {
            throw new NotImplementedException();
        }

        private void LoadXml()
        {
            var currentLocation = Path.GetDirectoryName(Assembly.GetAssembly(typeof(XmlConfig<TConfigElement>)).Location);
            xConfig = XDocument.Load(Path.Combine(currentLocation, FileName));
        }
    }
}
