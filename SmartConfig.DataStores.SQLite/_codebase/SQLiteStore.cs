using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Reusable;
using Reusable.Fuse;
using SmartConfig.Data;
using Reusable.Data;

namespace SmartConfig.DataStores.SQLite
{
    // ReSharper disable once InconsistentNaming
    public class SQLiteStore : DataStore
    {
        private Encoding _dataEncoding = Encoding.Default;
        private Encoding _settingEncoding = Encoding.UTF8;

        public SQLiteStore(string nameOrConnectionString, Action<TableConfigurationBuilder> tableConfigBuilder = null) : base(new[] { typeof(string) })
        {
            nameOrConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            ConnectionString = nameOrConnectionString;

            var connectionStringName = nameOrConnectionString.GetConnectionStringName();
            if (!string.IsNullOrEmpty(connectionStringName))
            {
                ConnectionString = new AppConfigRepository().GetConnectionString(connectionStringName);
            }

            ConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            var settingTableConfigBuilder = new TableConfigurationBuilder()
                .TableName(nameof(Setting))
                .Column(nameof(Setting.Name), DbType.String, 300)
                .Column(nameof(Setting.Value), DbType.String, ColumnConfiguration.MaxLength)
                .Column(SettingAttribute.Config);

            tableConfigBuilder?.Invoke(settingTableConfigBuilder);
            SettingTableConfiguration = settingTableConfigBuilder.Build();
        }

        public string ConnectionString { get; }

        public TableConfiguration SettingTableConfiguration { get; }

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

        public override IEnumerable<Setting> GetSettings(Setting setting)
        {
            var commandFactory = new CommandFactory(SettingTableConfiguration);

            using (var connection = new SQLiteConnection(ConnectionString))
            using (var command = commandFactory.CreateSelectCommand(connection, setting))
            {
                connection.Open();
                command.Prepare();

                using (var settingReader = command.ExecuteReader())
                {
                    while (settingReader.Read())
                    {
                        var value = (string)settingReader[nameof(Setting.Value)];

                        var result = new Setting
                        {
                            Name = SettingPath.Parse((string)settingReader[nameof(Setting.Name)]),
                            Value = RecodeDataEnabled ? value.Recode(DataEncoding, SettingEncoding) : value
                        };

                        foreach (var attribute in setting.Tags)
                        {
                            result[attribute.Key] = settingReader[attribute.Key];
                        }
                        yield return result;
                    }
                }
            }
        }

        public override int SaveSettings(IEnumerable<Setting> settings)
        {
            var groups = settings.GroupBy(x => x.Name.WeakFullName).ToList();
            if (!groups.Any())
            {
                return 0;
            }

            var commandFactory = new CommandFactory(SettingTableConfiguration);
            var rowsAffected = 0;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var group in groups)
                        {
                            var deleted = false;
                            foreach (var setting in group)
                            {
                                if (!deleted)
                                {
                                    using (var deleteCommand = commandFactory.CreateDeleteCommand(connection, setting))
                                    {
                                        deleteCommand.Transaction = transaction;
                                        deleteCommand.Prepare();
                                        rowsAffected += deleteCommand.ExecuteNonQuery();
                                    }
                                    deleted = true;
                                }

                                if (RecodeDataEnabled && setting.Value is string)
                                {
                                    setting.Value = ((string)setting.Value).Recode(SettingEncoding, DataEncoding);
                                }

                                using (var insertCommand = commandFactory.CreateInsertCommand(connection, setting))
                                {
                                    insertCommand.Transaction = transaction;
                                    insertCommand.Prepare();
                                    rowsAffected += insertCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                        return rowsAffected;
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
        // ReSharper disable once InconsistentNaming
        public static ConfigurationBuilder FromSQLite(
            this ConfigurationBuilder configurationBuilder,
            string nameOrConnectionString,
            Action<TableConfigurationBuilder> configure = null
        )
        {
            return configurationBuilder.From(new SQLiteStore(nameOrConnectionString, configure));
        }
    }
}
