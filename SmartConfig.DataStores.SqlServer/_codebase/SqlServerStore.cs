using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Reusable;
using Reusable.Data;
using SmartConfig.Data;
using SmartConfig.Collections;

namespace SmartConfig.DataStores.SqlServer
{
    /// <summary>
    /// Implements sql server data source.
    /// </summary>
    public class SqlServerStore : DataStore
    {
        private readonly SettingCommandFactory _settingCommandFactory;

        public SqlServerStore(string nameOrConnectionString, Action<TableMetadataBuilder<SqlDbType>> buildTableConfiguration = null) : base(new[] { typeof(string) })
        {            
            ConnectionString = nameOrConnectionString.NonEmptyOrNull() ?? throw new ArgumentNullException(nameof(nameOrConnectionString)); ;

            var connectionStringName = nameOrConnectionString.GetConnectionStringName();
            if (!string.IsNullOrEmpty(connectionStringName))
            {
                ConnectionString = new AppConfigRepository().GetConnectionString(connectionStringName).NonEmptyOrNull() ?? throw new ArgumentNullException(nameof(nameOrConnectionString)); ;
            }

            var tableMetadataBuilder = TableMetadataBuilder<SqlDbType>.Create()
                .SchemaName("dbo")
                .TableName(nameof(Setting))
                .Column(nameof(Setting.Name), SqlDbType.NVarChar, 300)
                .Column(nameof(Setting.Value), SqlDbType.NVarChar, -1);

            buildTableConfiguration?.Invoke(tableMetadataBuilder);
            TableMetadata = tableMetadataBuilder.Build();
            _settingCommandFactory = new SettingCommandFactory(TableMetadata);
        }

        public string ConnectionString { get; }

        public TableMetadata<SqlDbType> TableMetadata { get; }

        public override IEnumerable<Setting> ReadSettings(Setting setting)
        {
            using (var connection = OpenConnection())
            using (var command = _settingCommandFactory.CreateSelectCommand(connection, setting))
            {
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

        protected override void WriteSettings(ICollection<IGrouping<Setting, Setting>> settings)
        {
            void DeleteObsoleteSettings(SqlConnection connection, SqlTransaction transaction, IGrouping<Setting, Setting> obsoleteSettings)
            {
                using (var deleteCommand = _settingCommandFactory.CreateDeleteCommand(connection, obsoleteSettings.Key))
                {
                    deleteCommand.Transaction = transaction;
                    deleteCommand.Prepare();
                    deleteCommand.ExecuteNonQuery();
                }
            }

            void InsertNewSettings(SqlConnection connection, SqlTransaction transaction, IGrouping<Setting, Setting> newSettings)
            {
                foreach (var setting in newSettings)
                {
                    using (var insertCommand = _settingCommandFactory.CreateInsertCommand(connection, setting))
                    {
                        insertCommand.Transaction = transaction;
                        insertCommand.Prepare();
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }

            using (var connection = OpenConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var group in settings)
                    {
                        DeleteObsoleteSettings(connection, transaction, group);
                        InsertNewSettings(connection, transaction, group);                        
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

        private SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}

