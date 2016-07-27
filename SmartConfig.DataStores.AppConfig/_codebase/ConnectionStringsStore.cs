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
    public class ConnectionStringsStore : IDataStore
    {
        private readonly System.Configuration.Configuration _exeConfiguration;
        private readonly ConnectionStringsSection _connectionStringsSection;

        public ConnectionStringsStore()
        {
            _exeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _connectionStringsSection = _exeConfiguration.ConnectionStrings;
        }

        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(Setting setting)
        {
            var connectionStringSettings =
                _connectionStringsSection.ConnectionStrings.Cast<ConnectionStringSettings>()
                .Where(x => SettingPath.Parse(x.Name).IsLike(setting.Name) && !string.IsNullOrEmpty(x.ConnectionString))
                .Select(x => new Setting
                {
                    Name = x.Name,
                    Value = x.ConnectionString
                })
                .ToList();
            
            return connectionStringSettings;
        }

        public int SaveSetting(Setting setting)
        {
            return SaveSettings(new[] { setting });
        }

        public int SaveSettings(IReadOnlyCollection<Setting> settings)
        {
            var affectedSettings = 0;

            // remove connection strings
            var removeKeys = settings.Select(x => x.Name.FullName).Distinct().ToList();
            var removeConfigurationStrings =
                    _connectionStringsSection.ConnectionStrings.Cast<ConnectionStringSettings>()
                        .Where(x => removeKeys.Contains(x.Name, StringComparer.OrdinalIgnoreCase))
                        .ToList();

            foreach (var item in removeConfigurationStrings)
            {
                _connectionStringsSection.ConnectionStrings.Remove(item.Name);
            }

            foreach (var setting in settings)
            {
                var connectionStringSettings = _connectionStringsSection.ConnectionStrings[setting.Name.FullNameEx];
                if (connectionStringSettings == null)
                {
                    connectionStringSettings = new ConnectionStringSettings(setting.Name.FullNameEx, (string)setting.Value);
                    _connectionStringsSection.ConnectionStrings.Add(connectionStringSettings);
                }
                else
                {
                    connectionStringSettings.ConnectionString = (string)setting.Value;
                }
                affectedSettings++;
            }
            _exeConfiguration.Save(ConfigurationSaveMode.Minimal);
            return affectedSettings;
        }
    }
}
