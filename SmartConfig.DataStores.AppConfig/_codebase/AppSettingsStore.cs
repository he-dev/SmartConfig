using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.DataStores.AppConfig
{
    public class AppSettingsStore : DataStore
    {
        //private readonly System.Configuration.Configuration _exeConfiguration;
        //private readonly AppSettingsSection _appSettingsSection;

        public AppSettingsStore() : base(new[] { typeof(string) })
        {
            //_exeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);            
            //_appSettingsSection = _exeConfiguration.AppSettings;
        }

        public override IEnumerable<Setting> ReadSettings(Setting setting)
        {
            var exeConfig = OpenExeConfiguration();
            var keys = exeConfig.AppSettings.Settings.AllKeys.Like(setting);
            foreach (var key in keys)
            {
                yield return new Setting
                {
                    Name = SettingPath.Parse(key),
                    Value = exeConfig.AppSettings.Settings[key].Value
                };
            }
        }

        protected override void WriteSettings(ICollection<IGrouping<Setting, Setting>> settings)
        {
            var exeConfig = OpenExeConfiguration();

            // If we are saving an itemized setting its keys might have changed.
            // Since we don't know the old keys we need to delete all keys that are alike first.

            foreach (var group in settings)
            {
                DeleteSettingGroup(group, exeConfig.AppSettings);

                foreach (var setting in group)
                {
                    exeConfig.AppSettings.Settings.Add(setting.Name.StrongFullName, (string)setting.Value);
                }
            }
            exeConfig.Save(ConfigurationSaveMode.Minimal);
        }

        private void DeleteSettingGroup(IGrouping<Setting, Setting> settingGroup, AppSettingsSection appSettings)
        {
            var settingWeakPath = settingGroup.Key.Name;
            var keys = appSettings.Settings.AllKeys.Where(key => SettingPath.Parse(key).IsLike(settingWeakPath));
            foreach (var key in keys)
            {
                appSettings.Settings.Remove(key);
            }
        }

        private System.Configuration.Configuration OpenExeConfiguration() => ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    }
}
