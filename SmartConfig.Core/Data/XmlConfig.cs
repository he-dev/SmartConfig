using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using SmartUtilities;

namespace SmartConfig.Data
{
    public class XmlConfig<TSetting> : DataSource<TSetting> where TSetting : Setting, new()
    {
        private const string DefaultRootElementName = "smartConfig";
        private const string DefaultSettingElementName = "setting";

        private string _rootElementName;
        private string _settingElementName;

        public string FileName { get; set; }

        public string RootElementName
        {
            get { return _rootElementName ?? DefaultRootElementName; }
            set { _rootElementName = value; }
        }

        public string SettingElementName
        {
            get { return _settingElementName ?? DefaultSettingElementName; }
            set { _settingElementName = value; }
        }

        private string FullName
        {
            get
            {
                if (Path.IsPathRooted(FileName))
                {
                    return FileName;
                }

                var currentLocation = Path.GetDirectoryName(Assembly.GetAssembly(typeof(XmlConfig<TSetting>)).Location);
                // ReSharper disable once AssignNullToNotNullAttribute
                var fullName = Path.Combine(currentLocation, FileName);
                return fullName;
            }
        }

        public override string Select(string defaultKey)
        {
            var xConfig = XDocument.Load(FullName);

            var compositeKey = new CompositeKey(defaultKey, KeyNames, KeyProperties);

            var defaultKeyXPath = "//$rootElementName/$settingElementName[@$attributeName='$attributeValue']".FormatWith(new
            {
                RootElementName,
                SettingElementName,
                attributeName = EncodeKeyName(KeyNames.DefaultKeyName),
                attributeValue = compositeKey[KeyNames.DefaultKeyName]
            }, true);

            var xSettings = xConfig.XPathSelectElements(defaultKeyXPath);

            // create TSetting from each item
            var elements = xSettings.Select(x =>
            {
                // set default key and value
                var element = new TSetting
                {
                    Name = x.Attribute(EncodeKeyName(KeyNames.DefaultKeyName)).Value,
                    Value = x.Value
                };

                // set other keys
                foreach (var keyName in KeyNamesWithoutDefault)
                {
                    var attr = x.Attribute(EncodeKeyName(keyName));

                    // use the attribute value or an asterisk if attribute not found
                    element[keyName] = (attr == null ? Wildcards.Asterisk : attr.Value);
                }
                return element;
            }).ToList() as IEnumerable<TSetting>;

            elements = ApplyFilters(elements, compositeKey);
            var result = elements.FirstOrDefault();
            return result != null ? result.Value : null;
        }

        public override void Update(string defaultKey, string value)
        {
            var xConfig = XDocument.Load(FullName);

            var compositeKey = new CompositeKey(defaultKey, KeyNames, KeyProperties);

            // try get item from loaded config
            var attributeCondition = compositeKey.Select(x => "@$attributeName = '$attributeValue'".FormatWith(new
            {
                attributeName = EncodeKeyName(x.Key),
                attributeValue = x.Value
            }, true));

            var attributeConditions = string.Join(" and ", attributeCondition);

            var settingXPath = "//$rootElementName/$settingElementName[$attributeConditions]".FormatWith(new
            {
                RootElementName,
                SettingElementName,
                attributeConditions = EncodeKeyName(KeyNames.DefaultKeyName)
            }, true);

            var xSettings = xConfig.XPathSelectElements(settingXPath);
            var xSetting = xSettings.SingleOrDefault();

            // add new setting
            if (xSetting == null)
            {
                xSetting = new XElement(DefaultSettingElementName, value);
                xConfig.Add(xSetting);
            }

            // set custom keys
            foreach (var x in compositeKey)
            {
                xSetting.Add(new XAttribute(EncodeKeyName(x.Key), x.Value));
            }

            xConfig.Save(FullName);
        }

        private static string EncodeKeyName(string keyName)
        {
            // any uppercase character that is not followed by an uppercase character
            keyName = Regex.Replace(keyName, "([A-Z])(?![A-Z])", "-$1").ToLower();
            return keyName.TrimStart('-');
        }

    }
}
