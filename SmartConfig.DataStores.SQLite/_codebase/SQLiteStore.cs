using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Reusable;
using Reusable.Data;
using Reusable.Extensions;
using Reusable.Validations;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SQLite
{
    // ReSharper disable once InconsistentNaming
    public class SQLiteStore : IDataStore
    {
        private Encoding _dataEncoding = Encoding.Default;
        private Encoding _settingEncoding = Encoding.UTF8;

        public SQLiteStore(string nameOrConnectionString, Action<SettingTableConfiguration.Builder> configure = null)
        {
            nameOrConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            ConnectionString = nameOrConnectionString;

            var connectionStringName = nameOrConnectionString.ToConnectionStringName();
            if (!string.IsNullOrEmpty(connectionStringName))
            {
                ConnectionString = AppConfigRepository.GetConnectionString(connectionStringName);
            }

            ConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            var settingTablePropertiesBuilder = new SettingTableConfiguration.Builder();
            configure?.Invoke(settingTablePropertiesBuilder);
            SettingTableConfiguration = settingTablePropertiesBuilder.Build();
        }

        internal AppConfigRepository AppConfigRepository { get; } = new AppConfigRepository();

        public string ConnectionString { get; }

        public SettingTableConfiguration SettingTableConfiguration { get; }

        // ReSharper disable once InconsistentNaming
        public bool RecodeDataEnabled { get; set; } = true;

        public Encoding DataEncoding
        {
            get { return _dataEncoding; }
            set { _dataEncoding = value.Validate(nameof(DataEncoding)).IsNotNull().Value; }
        }

        public Encoding SettingEncoding
        {
            get { return _settingEncoding; }
            set { _settingEncoding = value.Validate(nameof(SettingEncoding)).IsNotNull().Value; }
        }

        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(Setting setting)
        {
            var commandFactory = new CommandFactory(SettingTableConfiguration);

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

            var commandFactory = new CommandFactory(SettingTableConfiguration);

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

    public static class ConfigurationBuilderExtensions
    {
        public static Configuration.Builder FromSQLite(
            this Configuration.Builder configurationBuilder,
            string nameOrConnectionString,
            Action<SettingTableConfiguration.Builder> configure = null
        )
        {
            return configurationBuilder.From(new SQLiteStore(nameOrConnectionString, configure));
        }
    }
}
