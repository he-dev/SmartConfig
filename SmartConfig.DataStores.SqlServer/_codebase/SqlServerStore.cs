using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Reusable;
using Reusable.Data;
using SmartConfig.Data;
using Reusable.Fuse;

namespace SmartConfig.DataStores.SqlServer
{
    /// <summary>
    /// Implements sql server data source.
    /// </summary>
    public class SqlServerStore : DataStore
    {
        public SqlServerStore(string nameOrConnectionString, Action<TableConfigurationBuilder> buildTableConfiguration = null) : base(new[] { typeof(string) })
        {
            nameOrConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            ConnectionString = nameOrConnectionString;

            var connectionStringName = nameOrConnectionString.GetConnectionStringName();
            if (!string.IsNullOrEmpty(connectionStringName))
            {
                ConnectionString = new AppConfigRepository().GetConnectionString(connectionStringName);
            }

            ConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            var settingTableConfigurationBuilder = new TableConfigurationBuilder()
                .SchemaName("dbo")
                .TableName(nameof(Setting))
                .Column(nameof(Setting.Name), SqlDbType.NVarChar, 300)
                .Column(nameof(Setting.Value), SqlDbType.NVarChar, ColumnConfiguration.MaxLength)
                .Column(SettingAttribute.Config);

            buildTableConfiguration?.Invoke(settingTableConfigurationBuilder);
            TableConfiguration = settingTableConfigurationBuilder.Build();
        }

        public string ConnectionString { get; }

        public TableConfiguration TableConfiguration { get; }

        public Type MapDataType(Type settingType) => typeof(string);

        public override IEnumerable<Setting> GetSettings(Setting setting)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var commandFactory = new CommandFactory(TableConfiguration);
                using (var command = commandFactory.CreateSelectCommand(connection, setting))
                {
                    connection.Open();
                    command.Prepare();

                    using (var settingReader = command.ExecuteReader())
                    {
                        while (settingReader.Read())
                        {
                            var result = new Setting
                            {
                                Name = SettingUrn.Parse((string)settingReader[nameof(Setting.Name)]),
                                Value = settingReader[nameof(Setting.Value)]
                            };
                            foreach (var attribute in setting.Attributes)
                            {
                                result[attribute.Key] = settingReader[attribute.Key];
                            }
                            yield return result;
                        }
                    }
                }
            }
        }

        public override int SaveSettings(IEnumerable<Setting> settings)
        {
            var groups = settings.GroupBy(x => x.WeakId).ToList();
            if (!groups.Any())
            {
                return 0;
            }

            var commandFactory = new CommandFactory(TableConfiguration);
            var rowsAffected = 0;

            using (var connection = new SqlConnection(ConnectionString))
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
                                // Before adding this group of settings delete the old ones first.
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
        public static ConfigurationBuilder FromSqlServer(
            this ConfigurationBuilder configurationBuilder,
            string nameOrConnectionString,
            Action<TableConfigurationBuilder> configure = null
        )
        {
            return configurationBuilder.From(new SqlServerStore(nameOrConnectionString, configure));
        }
    }
}

