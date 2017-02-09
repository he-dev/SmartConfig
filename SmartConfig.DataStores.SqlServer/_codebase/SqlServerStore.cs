using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Reusable;
using Reusable.Data;
using SmartConfig.Data;
using Reusable.Fuse;
using SmartConfig.Collections;

namespace SmartConfig.DataStores.SqlServer
{
    /// <summary>
    /// Implements sql server data source.
    /// </summary>
    public class SqlServerStore : DataStore
    {
        public SqlServerStore(string nameOrConnectionString, Action<TableMetadataBuilder<SqlDbType>> buildTableConfiguration = null) : base(new[] { typeof(string) })
        {
            nameOrConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            ConnectionString = nameOrConnectionString;

            var connectionStringName = nameOrConnectionString.GetConnectionStringName();
            if (!string.IsNullOrEmpty(connectionStringName))
            {
                ConnectionString = new AppConfigRepository().GetConnectionString(connectionStringName);
            }

            ConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            var tableMetadataBuilder = TableMetadataBuilder<SqlDbType>.Create()
                .SchemaName("dbo")
                .TableName(nameof(Setting))
                .Column(nameof(Setting.Name), SqlDbType.NVarChar, 300)
                .Column(nameof(Setting.Value), SqlDbType.NVarChar, -1)
                .Column(SettingTag.Config, SqlDbType.NVarChar, 50);

            buildTableConfiguration?.Invoke(tableMetadataBuilder);
            TableMetadata = tableMetadataBuilder.Build();
        }

        public string ConnectionString { get; }

        public TableMetadata<SqlDbType> TableMetadata { get; }

        public Type MapDataType(Type settingType) => typeof(string);

        public override IEnumerable<Setting> ReadSettings(Setting setting)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var commandFactory = new SettingCommandFactory(TableMetadata);
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
                                Name = SettingPath.Parse((string)settingReader[nameof(Setting.Name)]),
                                Value = settingReader[nameof(Setting.Value)],
                                Tags = new TagCollection(setting.Tags.ToDictionary(tag => tag.Key, tag => settingReader[tag.Key]))
                            };                            
                            yield return result;
                        }
                    }
                }
            }
        }

        protected override void WriteSettings(ICollection<IGrouping<Setting, Setting>> settings)
        {            
            var commandFactory = new SettingCommandFactory(TableMetadata);

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var group in settings)
                        {
                            using (var deleteCommand = commandFactory.CreateDeleteCommand(connection, group.Key))
                            {
                                deleteCommand.Transaction = transaction;
                                deleteCommand.Prepare();
                                deleteCommand.ExecuteNonQuery();
                            }

                            foreach (var setting in group)
                            {
                                using (var insertCommand = commandFactory.CreateInsertCommand(connection, setting))
                                {
                                    insertCommand.Transaction = transaction;
                                    insertCommand.Prepare();
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                        transaction.Commit();
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

