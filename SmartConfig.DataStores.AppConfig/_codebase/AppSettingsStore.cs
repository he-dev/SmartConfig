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

        public List<Setting> GetSettings(Setting setting)
        {
            var keys =
                _appSettingsSection.Settings.AllKeys
                .Where(k => SettingPath.Parse(k).IsLike(setting.Name))
                .ToArray();

            var settings = keys.Select(k => new Setting
            {
                Name = k,
                Value = _appSettingsSection.Settings[k].Value
            })
            .ToList();

            return settings;
        }

        public int SaveSetting(Setting setting)
        {
            return SaveSettings(new[] { setting });
        }

        public int SaveSettings(IReadOnlyCollection<Setting> settings)
        {
            var affectedSettings = 0;

            // remove settings
            var removeKeys = settings.Select(x => x.Name.FullName).Distinct().ToList();
            var removeConfigurationElements =
                    _appSettingsSection.Settings.Cast<KeyValueConfigurationElement>()
                        .Where(x => removeKeys.Contains(x.Key, StringComparer.OrdinalIgnoreCase))
                        .ToList();

            foreach (var configurationElement in removeConfigurationElements)
            {
                _appSettingsSection.Settings.Remove(configurationElement.Key);
            }

            foreach (var setting in settings)
            {
                var configurationElement = _appSettingsSection.Settings[setting.Name.FullNameEx];
                if (configurationElement == null)
                {
                    _appSettingsSection.Settings.Add(setting.Name.FullNameEx, (string)setting.Value);
                }
                else
                {
                    configurationElement.Value = (string)setting.Value;
                }
                affectedSettings++;
            }
            _exeConfiguration.Save(ConfigurationSaveMode.Minimal);
            return affectedSettings;
        }
    }
}
