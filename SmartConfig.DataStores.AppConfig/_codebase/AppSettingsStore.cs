using System;
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
            var rowsAffected = 0;

            // If we are saving an itemized setting its keys might have changed.
            // Since we don't know the old keys we need to delete all keys that are alike first.

            var groups = settings.GroupBy(x => x.WeakId).ToList();
            if (!groups.Any())
            {
                return rowsAffected;
            }

            foreach (var group in groups)
            {
                var groupDeleted = false;

                foreach (var setting in group)
                {
                    if (!groupDeleted)
                    {
                        // Get weak keys for this setting group to delete.
                        var keys = _appSettingsSection.Settings.AllKeys.Where(key => SettingUrn.Parse(key).IsLike(setting.Name)).ToList();
                        foreach (var key in keys)
                        {
                            _appSettingsSection.Settings.Remove(key);
                        }
                        groupDeleted = true;
                    }

                    // There shouldn't any key with this strong name any more so it's save to add it.
                    _appSettingsSection.Settings.Add(setting.Name.StrongFullName, (string)setting.Value);

                    rowsAffected++;

                }
            }
            _exeConfiguration.Save(ConfigurationSaveMode.Minimal);
            return rowsAffected;
        }
    }

    //var configurationElement = _appSettingsSection.Settings[setting.Name.StrongFullName];
    //                if (configurationElement == null)
    //                {
    //                    _appSettingsSection.Settings.Add(setting.Name.StrongFullName, (string)setting.Value);
    //                }
    //                else
    //                {
    //                    configurationElement.Value = (string)setting.Value;
    //                }
}
