using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.DataStores.AppConfig
{
    /// <summary>
    /// Implements the app.configuration as data source.
    /// </summary>
    public class AppSettingsStore : IDataStore
    {
        private readonly System.Configuration.Configuration _exeConfiguration;
        private readonly AppSettingsSection _appSettingsSection;

        public AppSettingsStore()
        {
            _exeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _appSettingsSection = _exeConfiguration.AppSettings;
        }

        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(SettingPath path, IReadOnlyDictionary<string, object> namespaces)
        {
            var result = new List<Setting>
            {
                new Setting
                {
                    Name = path.ToString(),
                   Value = _appSettingsSection.Settings[path.ToString()]?.Value
                }
            };
            return result;
        }

        public int SaveSetting(SettingPath path, IReadOnlyDictionary<string, object> namespaces, object value)
        {
            return SaveSettings(new Dictionary<SettingPath, object> { [path] = value }, namespaces);
        }

        public int SaveSettings(IReadOnlyDictionary<SettingPath, object> settings, IReadOnlyDictionary<string, object> namespaces)
        {
            var affectedSettings = 0;
            foreach (var setting in settings)
            {
                var element = _appSettingsSection.Settings[setting.Key.ToString()];
                if (element == null)
                {
                    _appSettingsSection.Settings.Add(setting.Key.ToString(), (string)setting.Value);
                }
                else
                {
                    element.Value = (string)setting.Value;
                }
                affectedSettings++;
            }
            _exeConfiguration.Save(ConfigurationSaveMode.Minimal);
            return affectedSettings;
        }
    }
}
