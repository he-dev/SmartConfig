using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using SmartConfig.Logging;
using SmartUtilities;

namespace SmartConfig.Data
{
    public class XmlSource<TSetting> : DataSource<TSetting> where TSetting : Setting, new()
    {
        private const string RootElementName = "smartConfig";
        private const string SettingElementName = "setting";

        private XDocument _xConfig;

        public XmlSource(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { throw new ArgumentNullException(nameof(fileName)); }

            if (!Path.IsPathRooted(fileName))
            {
                fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
            }

            if (!File.Exists(fileName)) { throw new FileNotFoundException(null, fileName); }

            FileName = fileName;
            _xConfig = XDocument.Load(FileName);
            if (_xConfig.Root == null || _xConfig.Root.Name != RootElementName) { throw new ArgumentException(null, nameof(fileName)); }
        }

        public string FileName { get; }

        public override string Select(IReadOnlyCollection<SettingKey> keys)
        {
            //var attributeName = EncodeKeyName(SettingKeyNames.DefaultKeyName);
            //var attributeValue = compositeKey[SettingKeyNames.DefaultKeyName];
            //var defaultKeyXPath = $"//{RootElementName}/{SettingElementName}[@{attributeName}='{attributeValue}']";

            //var xSettings = _xConfig.XPathSelectElements(defaultKeyXPath);

            //// create TSetting from each item
            //var elements = xSettings.Select(x =>
            //{
            //    // set default key and value
            //    var element = new TSetting
            //    {
            //        Name = x.Attribute(EncodeKeyName(SettingKeyNames.DefaultKeyName)).Value,
            //        Value = x.Value
            //    };

            //    // set other keys
            //    foreach (var keyName in KeyNamesWithoutDefault)
            //    {
            //        var attr = x.Attribute(EncodeKeyName(keyName));

            //        // use the attribute value or an asterisk if attribute not found
            //        element[keyName] = attr?.Value ?? Wildcards.Asterisk;
            //    }
            //    return element;
            //}).ToList() as IEnumerable<TSetting>;

            //elements = ApplyFilters(elements, compositeKey);
            //var result = elements.FirstOrDefault();
            //return result?.Value;

            return null;
        }

        public override void Update(IReadOnlyCollection<SettingKey> keys, string value)
        {
            //var compositeKey = CreateCompositeKey(defaultKeyValue);

            //// try get item from loaded config
            //var attributeCondition = compositeKey.Select(x =>
            //{
            //    var attributeName = EncodeKeyName(x.Key);
            //    var attributeValue = x.Value;
            //    return $"@{attributeName} = '{attributeValue}'";
            //});

            //var attributeConditions = string.Join(" and ", attributeCondition);

            //var settingXPath = $"//{RootElementName}/{SettingElementName}[{attributeConditions}]";

            //var xSettings = _xConfig.XPathSelectElements(settingXPath);
            //var xSetting = xSettings.SingleOrDefault();

            //// add new setting
            //if (xSetting == null)
            //{
            //    xSetting = new XElement(
            //        SettingElementName,
            //        new XAttribute(EncodeKeyName(SettingKeyNames.DefaultKeyName), defaultKeyValue),
            //        value);
            //    _xConfig.Root.Add(xSetting);
            //}

            //// set custom keys
            //foreach (var keyName in KeyNamesWithoutDefault)
            //{
            //    xSetting.Add(new XAttribute(EncodeKeyName(keyName), compositeKey[keyName]));
            //}

            //xSetting.Value = value;

            //_xConfig.Save(FileName);
        }

        //internal static string EncodeKeyName(string keyName)
        //{
        //    // any uppercase character that is not followed by an uppercase character
        //    keyName = Regex.Replace(keyName, "([A-Z])(?![A-Z])", "-$1").ToLower();
        //    return keyName.TrimStart('-');
        //}
    }
}
