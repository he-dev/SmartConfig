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
    public class ConnectionStringsStore : DataStore
    {
        private readonly System.Configuration.Configuration _exeConfiguration;
        private readonly ConnectionStringsSection _connectionStringsSection;

        public ConnectionStringsStore() : base(new[] { typeof(string) })
        {
            _exeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _connectionStringsSection = _exeConfiguration.ConnectionStrings;
        }

        public Type MapDataType(Type settingType) => typeof(string);

        public override IEnumerable<Setting> GetSettings(Setting setting)
        {
            var connectionStringSettings =
                _connectionStringsSection.ConnectionStrings.Cast<ConnectionStringSettings>()
                .Where(x => SettingUrn.Parse(x.Name).IsLike(setting.Name) && !string.IsNullOrEmpty(x.ConnectionString))
                .Select(x => new Setting
                {
                    Name = x.Name,
                    Value = x.ConnectionString
                })
                .ToList();
            
            return connectionStringSettings;
        }      

        public override int SaveSettings(IEnumerable<Setting> settings)
        {
            var affectedSettings = 0;

            // remove connection strings
            var removeKeys = settings.Select(x => x.Name.WeakFullName).Distinct().ToList();
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
                var connectionStringSettings = _connectionStringsSection.ConnectionStrings[setting.Name.StrongFullName];
                if (connectionStringSettings == null)
                {
                    connectionStringSettings = new ConnectionStringSettings(setting.Name.StrongFullName, (string)setting.Value);
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
