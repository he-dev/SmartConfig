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

        public List<Setting> GetSettings(SettingPath path, IReadOnlyDictionary<string, object> namespaces)
        {
            var result = new List<Setting>
            {
                new Setting
                {
                    Name = new SettingPath(path),
                    Value = _connectionStringsSection.ConnectionStrings[path.ToString()]?.ConnectionString
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
                var connectionStringSettings = _connectionStringsSection.ConnectionStrings[setting.Key.ToString()];
                if (connectionStringSettings == null)
                {
                    connectionStringSettings = new ConnectionStringSettings(setting.Key.ToString(), (string)setting.Value);
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
