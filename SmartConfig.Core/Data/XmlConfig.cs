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

        public string FileName { get; set; }

        public override string Select(IDictionary<string, string> keys)
        {
            LoadXml();

            // find all elements where the name-attribute's value is equal to the specified key
            // the search is case insensitive
            var xElements =
                xConfig.Root.Elements()
                .Where(e => e.Attribute("name").Value.Equals(keys[SmartConfig.KeyNames.DefaultKeyName], StringComparison.OrdinalIgnoreCase));

            // create TConfigElement from each item
            var elements = xElements.Select(x =>
            {
                var element = new TConfigElement
                {
                    Name = x.Attribute("name").Value,
                    Value = x.Value
                };

                // set custom properties
                foreach (var keyName in OrderedKeyNames.Where(k => k != SmartConfig.KeyNames.DefaultKeyName))
                {
                    // find an attribute for the property - the search is case insensitive
                    var attr = x.Attributes().SingleOrDefault(a => a.Name.ToString().Equals(keyName, StringComparison.OrdinalIgnoreCase));

                    // use the attribute value or an asterisk if attribute not found
                    element.SetStringDelegates[keyName](attr == null ? Wildcards.Asterisk : attr.Value);
                }
                return element;
            });

            // apply filters for all keys except the default one
            elements = keys
                .Where(x => x.Key != SmartConfig.KeyNames.DefaultKeyName)
                .Aggregate(elements, (current, keyValue) => _keys[keyValue.Key].Filter(current, keyValue));

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
