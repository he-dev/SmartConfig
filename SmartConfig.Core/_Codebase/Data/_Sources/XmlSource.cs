using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using SmartConfig.Collections;
using SmartUtilities;

namespace SmartConfig.Data
{
    public class XmlSource<TSetting> : DataSource<TSetting> where TSetting : Setting, new()
    {
        private const string RootElementName = "SmartConfig";
        private const string SettingElementName = "Setting";

        public XmlSource(string fileName)
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

        private XElement Root => XConfig.Root;

        public override IReadOnlyCollection<Type> SupportedSettingValueTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKeyCollection keys)
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

        public override void Update(SettingKeyCollection keys, object value)
        {
            var attributeConditions = string.Join(" and ", keys.Select(key => $"@{key.Name} = '{key.Value}'"));
            var settingXPath = $"//{RootElementName}/{SettingElementName}[{attributeConditions}]";

            var xSettings = XConfig.XPathSelectElements(settingXPath);
            var xSetting = xSettings.SingleOrDefault();

            // add new setting
            if (xSetting == null)
            {
                xSetting = new XElement(SettingElementName, new XAttribute(Setting.DefaultKeyName, keys.First().Value), value);
                XConfig.Root.Add(xSetting);
            }

            // set custom keys
            foreach (var key in keys.CustomKeys)
            {
                xSetting.Add(new XAttribute(key.Name, key.Value));
            }

            xSetting.Value = value?.ToString();
            XConfig.Save(FileName);
        }
    }
}
