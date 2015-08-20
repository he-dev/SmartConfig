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
    public class XmlConfig<TSetting> : DataSource<TSetting> where TSetting : Setting, new()
    {
        private XDocument xConfig;

        public string FileName { get; set; }

        public override void InitializeSettings(IDictionary<string, string> values)
        {
        }

        public override string Select(string defaultKey)
        {
            LoadXml();

            var compositeKey = CompositeKey.From(defaultKey, KeyNames, KeyProperties);

            // find all elements where the name-attribute's value is equal to the specified key
            // the search is case insensitive
            var xElements =
                xConfig.Root.Elements()
                .Where(e => e.Attribute("name").Value.Equals(compositeKey[KeyNames.DefaultKeyName], StringComparison.OrdinalIgnoreCase));

            // create TConfigElement from each item
            var elements = xElements.Select(x =>
            {
                var element = new TSetting
                {
                    Name = x.Attribute("name").Value,
                    Value = x.Value
                };

                // set custom properties
                foreach (var keyName in KeyNames.Where(k => k != KeyNames.DefaultKeyName))
                {
                    // find an attribute for the property - the search is case insensitive
                    var attr = x.Attributes().SingleOrDefault(a => a.Name.ToString().Equals(keyName, StringComparison.OrdinalIgnoreCase));

                    // use the attribute value or an asterisk if attribute not found
                    element[keyName] = (attr == null ? Wildcards.Asterisk : attr.Value);
                }
                return element;
            });

            // apply filters for all keys except the default one
            elements = ApplyFilters(elements, compositeKey);

            var result = elements.FirstOrDefault();
            return result != null ? result.Value : null;
        }

        public override void Update(string defaultKey, string value)
        {
            throw new NotImplementedException();
        }

        private void LoadXml()
        {
            var currentLocation = Path.GetDirectoryName(Assembly.GetAssembly(typeof(XmlConfig<TSetting>)).Location);
            xConfig = XDocument.Load(Path.Combine(currentLocation, FileName));
        }
    }
}
