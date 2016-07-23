using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig.DataStores.XmlFile
{
    public class XmlFileStore : IDataStore
    {
        private const string RootElementName = "SmartConfig";
        private const string SettingElementName = "Setting";

        public XmlFileStore(string fileName)
        {
            fileName.Validate(nameof(fileName)).IsNotNullOrEmpty();
            Path.IsPathRooted(fileName).Validate(nameof(fileName)).IsTrue(ctx => $"File name path must be rooted.");
            File.Exists(fileName).Validate(nameof(fileName)).IsTrue(ctx => $"'{fileName}' not found.");

            FileName = fileName;
            XConfig = XDocument.Load(FileName);

            XConfig.Root.Validate(nameof(fileName))
                .IsNotNull(ctx => "Root element not found.")
                .And(x => x.Name.ToString())
                .IsEqual(RootElementName, ctx => $"Invalid root element name '{ctx.Argument}'. Expected '{RootElementName}'");
        }

        public string FileName { get; }

        private XDocument XConfig { get; }

        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(SettingPath path, IReadOnlyDictionary<string, object> namespaces)
        {
            var attributes = namespaces.Aggregate(
                $"@{nameof(Setting.Name)}='{path}'",
                (result, next) => $"{result} and @{next.Key}='{next.Value}'");

            var xPath = $"//{RootElementName}/{SettingElementName}[{attributes}]";
            var xSettings = XConfig.XPathSelectElements(xPath);

            var elements = xSettings.Select(x =>
            {
                // set default key and value
                var element = new Setting
                {
                    Name = x.Attribute(nameof(Setting.Name)).Value,
                    Value = x.Value
                };

                // set other keys
                foreach (var settingNamespace in namespaces)
                {
                    element[settingNamespace.Key] = x.Attribute(settingNamespace.Key).Value;
                }
                return element;
            }).ToList();

            return elements;
        }

        public int SaveSetting(SettingPath path, IReadOnlyDictionary<string, object> namespaces, object value)
        {
            throw new NotImplementedException();
        }

        public int SaveSettings(IReadOnlyDictionary<SettingPath, object> settings, IReadOnlyDictionary<string, object> namespaces)
        {
            var affectedSettings = 0;
            foreach (var setting in settings)
            {
                var xSettings = GetXSettings(setting.Key, namespaces).ToList();
                xSettings.Count
                    .Validate(setting.Key)
                    .IsTrue(x => x <= 1, ctx => $"'{ctx.MemberName}' found more the once.");

                var xSetting = xSettings.SingleOrDefault();

                // add new setting
                if (xSetting == null)
                {
                    xSetting = new XElement(
                        SettingElementName,
                        new XAttribute(nameof(Setting.Name), setting.Key), setting.Value);

                    foreach (var settingNamespace in namespaces)
                    {
                        xSetting.Add(new XAttribute(settingNamespace.Key, settingNamespace.Value));
                    }

                    // ReSharper disable once PossibleNullReferenceException
                    XConfig.Root.Add(xSetting);
                }
                xSetting.Value = setting.Value.ToString();
                affectedSettings++;
            }

            XConfig.Save(FileName);
            return affectedSettings;
        }

        private IEnumerable<XElement> GetXSettings(SettingPath name, IReadOnlyDictionary<string, object> namespaces)
        {
            var attributes = namespaces.Aggregate(
                $"@{nameof(Setting.Name)}='{name}'",
                (result, next) => $"{result} and {next.Key}='{next.Value}'");

            var xPath = $"//{RootElementName}/{SettingElementName}[{attributes}]";
            var xSettings = XConfig.XPathSelectElements(xPath);
            return xSettings;
        }
    }
}
