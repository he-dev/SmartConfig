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
    // ReSharper disable once InconsistentNaming
    public class SQLiteStore : IDataStore
    {
        private Encoding _dataEncoding = Encoding.Default;
        private Encoding _settingEncoding = Encoding.UTF8;

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

        // ReSharper disable once InconsistentNaming
        public bool RecodeDataEnabled { get; set; } = true;

        public Encoding DataEncoding
        {
            get { return _dataEncoding; }
            set { _dataEncoding = value.Validate(nameof(DataEncoding)).IsNotNull().Argument; }
        }

        public Encoding SettingEncoding
        {
            get { return _settingEncoding; }
            set { _settingEncoding = value.Validate(nameof(SettingEncoding)).IsNotNull().Argument; }
        }

        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(Setting setting)
        {
            var commandFactory = new CommandFactory(SettingTableProperties);

            using (var connection = new SQLiteConnection(ConnectionString))
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
                            Value = RecodeDataEnabled ? value.Recode(DataEncoding, SettingEncoding) : value
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

            var settingGroups = settings.GroupBy(x => x.Name.FullName).ToList();

            // All settings have the same columns so we can use the first setting to create commands.
            var setting0 = settings.First();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                using (var insertCommand = commandFactory.CreateInsertCommand(connection, setting0))
                using (var deleteCommand = commandFactory.CreateDeleteCommand(connection, setting0))
                {
                    deleteCommand.Transaction = transaction;
                    deleteCommand.Prepare();

                    insertCommand.Transaction = transaction;
                    insertCommand.Prepare();

                    try
                    {
                        var affectedSettings = 0;

                        // delete

                        foreach (var sg in settingGroups)
                        {
                            var s0 = sg.First();

                            // Delete all settings for this group.
                            deleteCommand.Parameters[nameof(Setting.Name)].Value = s0.Name.FullName;
                            foreach (var ns in s0.Namespaces) { deleteCommand.Parameters[ns.Key].Value = ns.Value; }
                            affectedSettings += deleteCommand.ExecuteNonQuery();

                            foreach (var s in sg)
                            {
                                insertCommand.Parameters[nameof(Setting.Name)].Value = s.Name.FullNameEx;
                                insertCommand.Parameters[nameof(Setting.Value)].Value =
                                    RecodeDataEnabled && s.Value is string
                                    ? ((string)s.Value).Recode(SettingEncoding, DataEncoding)
                                    : s.Value;

                                foreach (var ns in s.Namespaces) { insertCommand.Parameters[ns.Key].Value = ns.Value; }
                                affectedSettings += insertCommand.ExecuteNonQuery();
                            }
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
