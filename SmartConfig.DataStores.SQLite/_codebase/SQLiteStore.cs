using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Reusable;
using SmartConfig.Data;
using Reusable.Data;
using SmartConfig.Collections;

namespace SmartConfig.DataStores.SQLite
{
    // ReSharper disable once InconsistentNaming
    public class SQLiteStore : DataStore
    {
        private Encoding _dataEncoding = Encoding.Default;
        private Encoding _settingEncoding = Encoding.UTF8;
        private readonly SettingCommandFactory _settingCommandFactory;

        public SQLiteStore(string nameOrConnectionString, Action<TableMetadataBuilder<DbType>> tableConfigBuilder = null) : base(new[] { typeof(string) })
        {
            ConnectionString = nameOrConnectionString.NonEmptyOrNull() ?? throw new ArgumentNullException(nameof(nameOrConnectionString));

            var connectionStringName = nameOrConnectionString.GetConnectionStringName();
            if (!string.IsNullOrEmpty(connectionStringName))
            {
                ConnectionString = (new AppConfigRepository().GetConnectionString(connectionStringName)).NonEmptyOrNull() ?? throw new ArgumentNullException(nameof(nameOrConnectionString)); ;
            }

            var settingTableConfigBuilder =
                TableMetadataBuilder<DbType>.Create()
                    .TableName(nameof(Setting))
                    .Column(nameof(Setting.Name), DbType.String, 300)
                    .Column(nameof(Setting.Value), DbType.String, -1)
                    .Column(SettingTag.Config, DbType.String, 50);

            tableConfigBuilder?.Invoke(settingTableConfigBuilder);
            SettingTableMetadata = settingTableConfigBuilder.Build();
            _settingCommandFactory = new SettingCommandFactory(SettingTableMetadata);
        }

        public string ConnectionString { get; }

        public TableMetadata<DbType> SettingTableMetadata { get; }

        public bool CanRecodeData { get; set; } = true;

        public bool CanRecodeSetting { get; set; } = true;

        public Encoding DataEncoding
        {
            get { return _dataEncoding; }
            set { _dataEncoding = value ?? throw new ArgumentNullException(nameof(DataEncoding)); }
        }

        public Encoding SettingEncoding
        {
            get { return _settingEncoding; }
            set { _settingEncoding = value ?? throw new ArgumentNullException(nameof(SettingEncoding)); }
        }

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
                        var value = (string)settingReader[nameof(Setting.Value)];

                        var result = new Setting
                        {
                            Name = SettingPath.Parse((string)settingReader[nameof(Setting.Name)]),
                            Value = CanRecodeData ? value.Recode(DataEncoding, SettingEncoding) : value,
                            Tags = new TagCollection(setting.Tags.ToDictionary(tag => tag.Key, tag => settingReader[tag.Key]))
                        };
                        yield return result;
                    }
                }
            }
        }

        protected override void WriteSettings(ICollection<IGrouping<Setting, Setting>> settings)
        {


            void DeleteObsoleteSettings(SQLiteConnection connection, SQLiteTransaction transaction, IGrouping<Setting, Setting> obsoleteSettings)
            {
                using (var deleteCommand = _settingCommandFactory.CreateDeleteCommand(connection, obsoleteSettings.Key))
                {
                    deleteCommand.Transaction = transaction;
                    deleteCommand.Prepare();
                    deleteCommand.ExecuteNonQuery();
                }
            }

            void InsertNewSettings(SQLiteConnection connection, SQLiteTransaction transaction, IGrouping<Setting, Setting> newSettings)
            {
                foreach (var setting in newSettings)
                {
                    if (CanRecodeSetting && setting.Value is string)
                    {
                        setting.Value = ((string)setting.Value).Recode(SettingEncoding, DataEncoding);
                    }

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

        private SQLiteConnection OpenConnection()
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
