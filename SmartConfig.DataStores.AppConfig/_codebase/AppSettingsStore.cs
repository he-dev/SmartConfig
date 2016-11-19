﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using SmartConfig.Data;

namespace SmartConfig.DataStores.AppConfig
{
    /// <summary>
    /// Implements the app.configuration as data source.
    /// </summary>
    public class AppSettingsStore : DataStore
    {
        private readonly System.Configuration.Configuration _exeConfiguration;
        private readonly AppSettingsSection _appSettingsSection;

        public AppSettingsStore() : base(new[] { typeof(string) })
        {
            _exeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _appSettingsSection = _exeConfiguration.AppSettings;
        }

        public Type MapDataType(Type settingType) => typeof(string);

        public override IEnumerable<Setting> GetSettings(Setting setting)
        {
            var keys =
                _appSettingsSection.Settings.AllKeys
                .Where(k => SettingUrn.Parse(k).IsLike(setting.Name))
                .ToArray();

            var settings = keys.Select(k => new Setting
            {
                Name = k,
                Value = _appSettingsSection.Settings[k].Value
            })
            .ToList();

            return settings;
        }

        public override int SaveSettings(IEnumerable<Setting> settings)
        {
            var affectedSettings = 0;

            // If we are saving an itemized setting its keys might have changed.
            // Since we don't know the old keys we need to delete all keys that are alike first.

            var keys2Remove =
                _appSettingsSection.Settings.AllKeys.Where(
                    key => settings.Any(x => x.Name.IsLike(SettingUrn.Parse(key)))
                ).ToList();

            foreach (var k in keys2Remove)
            {
                _appSettingsSection.Settings.Remove(k);
            }

            // Now we can add the new settings.
            foreach (var setting in settings)
            {
                var configurationElement = _appSettingsSection.Settings[setting.Name.FullNameWithKey];
                if (configurationElement == null)
                {
                    _appSettingsSection.Settings.Add(setting.Name.FullNameWithKey, (string)setting.Value);
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
