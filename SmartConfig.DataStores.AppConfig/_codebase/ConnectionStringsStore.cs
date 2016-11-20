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

        public override IEnumerable<Setting> GetSettings(Setting setting)
        {
            var connectionStringSettings =
                _connectionStringsSection.ConnectionStrings.Cast<ConnectionStringSettings>()
                .Where(x => SettingUrn.Parse(x.Name).IsLike(setting.Name) && !string.IsNullOrEmpty(x.ConnectionString))
                .Select(x => new Setting
                {
                    Name = SettingUrn.Parse(x.Name),
                    Value = x.ConnectionString
                })
                .ToList();

            return connectionStringSettings;
        }

        public override int SaveSettings(IEnumerable<Setting> settings)
        {
            var rowsAffected = 0;

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
                        var names =
                            _connectionStringsSection.ConnectionStrings
                                .Cast<ConnectionStringSettings>()
                                .Where(x => SettingUrn.Parse(x.Name).IsLike(setting.Name));

                        foreach (var item in names)
                        {
                            _connectionStringsSection.ConnectionStrings.Remove(item.Name);
                        }
                        groupDeleted = true;
                    }

                    var connectionStringSettings = new ConnectionStringSettings(setting.Name.StrongFullName, (string)setting.Value);
                    _connectionStringsSection.ConnectionStrings.Add(connectionStringSettings);
                    rowsAffected++;
                }
            }
            
            _exeConfiguration.Save(ConfigurationSaveMode.Minimal);
            return rowsAffected;
        }
    }

    //var connectionStringSettings = _connectionStringsSection.ConnectionStrings[setting.Name.StrongFullName];
    //            if (connectionStringSettings == null)
    //            {
    //                connectionStringSettings = new ConnectionStringSettings(setting.Name.StrongFullName, (string)setting.Value);
    //                _connectionStringsSection.ConnectionStrings.Add(connectionStringSettings);
    //            }
    //            else
    //            {
    //                connectionStringSettings.ConnectionString = (string)setting.Value;
    //            }
}
