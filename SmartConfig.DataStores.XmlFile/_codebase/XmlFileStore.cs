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

        public override IReadOnlyCollection<Type> SupportedSettingDataTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(CompoundSettingKey keys)
        {
            var attributeName = keys.NameKey.Name;
            var attributeValue = keys.NameKey.Value;
            var defaultKeyXPath = $"//{RootElementName}/{SettingElementName}[@{attributeName}='{attributeValue}']";

            var xSettings = XConfig.XPathSelectElements(defaultKeyXPath);

            // create TSetting from each item
            var elements = xSettings.Select(x =>
            {
                // set default key and value
                var element = new TSetting
                {
                    Name = x.Attribute(keys.NameKey.Name).Value,
                    Value = x.Value
                };

                // set other keys
                foreach (var key in keys.CustomKeys)
                {
                    var attr = x.Attribute(key.Name);

                    // use the attribute value or an asterisk if attribute not found
                    element[key.Name] = attr?.Value ?? Wildcards.Asterisk;
                }
                return element;
            }).ToList() as IEnumerable<TSetting>;

            elements = ApplyFilters(elements, keys.Skip(1));
            var result = elements.FirstOrDefault();
            return result?.Value;
        }

        public override void Update(CompoundSettingKey keys, object value)
        {
            if (keys == null) { throw new ArgumentNullException(nameof(keys)); }
            if (value == null) { throw new ArgumentNullException(nameof(value)); }

            var attributeConditions = string.Join(" and ", keys.Select(key => $"@{key.Name} = '{key.Value}'"));
            var settingXPath = $"//{RootElementName}/{SettingElementName}[{attributeConditions}]";

            var xSettings = XConfig.XPathSelectElements(settingXPath);
            var xSetting = xSettings.SingleOrDefault();

            // add new setting
            if (xSetting == null)
            {
                xSetting = new XElement(
                    SettingElementName,
                    new XAttribute(keys.NameKey.Name, keys.NameKey.Value), value);

                foreach (var key in keys.CustomKeys)
                {
                    xSetting.Add(new XAttribute(key.Name, key.Value));
                }

                XConfig.Root.Add(xSetting);
            }
            else
            {
                // set custom keys
                foreach (var key in keys.CustomKeys)
                {
                    xSetting.Attribute(key.Name).Value = key.Value.ToString();
                }
            }

            xSetting.Value = value.ToString();
            XConfig.Save(FileName);
        }
    }
}
