using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartUtilities;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig.DataStores.SQLite
{
    public class SQLiteStore : IDataStore
    {
        public SQLiteStore(string nameOrConnectionString, Action<SettingTableProperties.Builder> buildSettingTableProperties = null)
        {
            nameOrConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            ConnectionString = nameOrConnectionString;

            string connectionStringName;
            if (nameOrConnectionString.TryGetConnectionStringName(out connectionStringName))
            {
                ConnectionString = AppConfigRepository.GetConnectionString(connectionStringName);
            }

            ConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            var settingTablePropertiesBuilder = new SettingTableProperties.Builder();
            buildSettingTableProperties?.Invoke(settingTablePropertiesBuilder);
            SettingTableProperties = settingTablePropertiesBuilder.ToSettingTableProperties();
        }

        internal AppConfigRepository AppConfigRepository { get; } = new AppConfigRepository();

        public string ConnectionString { get; }

        public SettingTableProperties SettingTableProperties { get; }

        public bool UTF8FixEnabled { get; set; } = true;

        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(Setting setting)
        {
            var commandFactory = new CommandFactory(SettingTableProperties);

            using (var connection = new SQLiteConnection(ConnectionString)) // "Data Source=config.db;Version=3;"))
            using (var command = commandFactory.CreateSelectCommand(connection, setting))
            {
                connection.Open();
                command.Prepare();

                using (var settingReader = command.ExecuteReader())
                {
                    var settings = new List<Setting>();

                    while (settingReader.Read())
                    {
                        var value = (string)settingReader[nameof(Setting.Value)];

                        var result = new Setting
                        {
                            Name = (string)settingReader[nameof(Setting.Name)],
                            Value = UTF8FixEnabled ? value.ToUTF8() : value
                        };

                        foreach (var ns in setting.Namespaces)
                        {
                            setting[ns.Key] = settingReader[ns.Key];
                        }
                        settings.Add(result);
                    }
                    return settings;
                }
            }
        }

        public int SaveSetting(Setting setting)
        {
            return SaveSettings(new[] { setting });
        }

        public int SaveSettings(IReadOnlyCollection<Setting> settings)
        {
            if (!settings.Any())
            {
                return 0;
            }

            var commandFactory = new CommandFactory(SettingTableProperties);

            var firstSetting = settings.First();

            using (var connection = new SQLiteConnection(ConnectionString)) // "Data Source=config.db;Version=3;"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                using (var command = commandFactory.CreateInsertCommand(connection, firstSetting))
                {
                    command.Transaction = transaction;
                    command.Prepare();

                    try
                    {
                        var affectedSettings = 0;

                        // delete

                        foreach (var setting in settings)
                        {
                            command.Parameters[nameof(Setting.Name)].Value = setting.Name.FullNameEx;
                            command.Parameters[nameof(Setting.Value)].Value = setting.Value;

                            foreach (var ns in setting.Namespaces)
                            {
                                command.Parameters[ns.Key].Value = ns.Value;
                            }

                            affectedSettings += command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        return affectedSettings;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
