using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.DataStores.XmlFile
{
    public class XmlFileStore<TSetting> : DataStore<TSetting> where TSetting : BasicSetting, new()
    {
        private const string RootElementName = "SmartConfig";
        private const string SettingElementName = "Setting";

        public XmlFileStore(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (!Path.IsPathRooted(fileName))
            {
                throw new FileNameNotRootedException { FileName = fileName };
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(null, fileName);
            }

            FileName = fileName;
            XConfig = XDocument.Load(FileName);
            if (XConfig.Root == null || XConfig.Root.Name != RootElementName)
            {
                throw new SmartConfigRootElementNotFountException { FileName = fileName };
            }
        }

        public string FileName { get; }

        private XDocument XConfig { get; }

        public override IReadOnlyCollection<Type> SerializationTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKey key)
        {
            var attributeName = key.Main.Key;
            var attributeValue = key.Main.Value.ToString();
            var defaultKeyXPath = $"//{RootElementName}/{SettingElementName}[@{attributeName}='{attributeValue}']";

            var xSettings = XConfig.XPathSelectElements(defaultKeyXPath);

            // create TSetting from each item
            var elements = xSettings.Select(x =>
            {
                // set default key and value
                var element = new TSetting
                {
                    Name = x.Attribute(key.Main.Key).Value,
                    Value = x.Value
                };

                // set other keys
                foreach (var custom in key.CustomKeys)
                {
                    var attr = x.Attribute(custom.Key);

                    // use the attribute value or an asterisk if attribute not found
                    element[custom.Key] = attr?.Value ?? Wildcards.Asterisk;
                }
                return element;
            }).ToList() as IEnumerable<TSetting>;

            elements = ApplyFilters(elements, key.CustomKeys);
            var result = elements.FirstOrDefault();
            return result?.Value;
        }

        public override void Update(SettingKey key, object value)
        {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            if (value == null) { throw new ArgumentNullException(nameof(value)); }

            var attributeConditions = string.Join(" and ", key.Select(custom => $"@{custom.Key} = '{custom.Value}'"));
            var settingXPath = $"//{RootElementName}/{SettingElementName}[{attributeConditions}]";

            var xSettings = XConfig.XPathSelectElements(settingXPath);
            var xSetting = xSettings.SingleOrDefault();

            // add new setting
            if (xSetting == null)
            {
                xSetting = new XElement(
                    SettingElementName,
                    new XAttribute(BasicSetting.MainKeyName, key.Main), value);

                foreach (var custom in key.CustomKeys)
                {
                    xSetting.Add(new XAttribute(custom.Key, custom.Value));
                }

                XConfig.Root.Add(xSetting);
            }
            else
            {
                // set custom keys
                foreach (var custom in key.CustomKeys)
                {
                    xSetting.Attribute(custom.Key).Value = custom.Value.ToString();
                }
            }

            xSetting.Value = value.ToString();
            XConfig.Save(FileName);
        }
    }
}
