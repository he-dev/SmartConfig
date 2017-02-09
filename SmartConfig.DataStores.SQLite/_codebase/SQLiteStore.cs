﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Reusable;
using Reusable.Fuse;
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

        public SQLiteStore(string nameOrConnectionString, Action<TableMetadataBuilder<DbType>> tableConfigBuilder = null) : base(new[] { typeof(string) })
        {
            nameOrConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            ConnectionString = nameOrConnectionString;

            var connectionStringName = nameOrConnectionString.GetConnectionStringName();
            if (!string.IsNullOrEmpty(connectionStringName))
            {
                ConnectionString = new AppConfigRepository().GetConnectionString(connectionStringName);
            }

            ConnectionString.Validate(nameof(nameOrConnectionString)).IsNotNullOrEmpty();

            var settingTableConfigBuilder = 
                TableMetadataBuilder<DbType>.Create()
                    .TableName(nameof(Setting))
                    .Column(nameof(Setting.Name), DbType.String, 300)
                    .Column(nameof(Setting.Value), DbType.String, -1)
                    .Column(SettingTag.Config, DbType.String, 50);

            tableConfigBuilder?.Invoke(settingTableConfigBuilder);
            SettingTableMetadata = settingTableConfigBuilder.Build();
        }

        public string ConnectionString { get; }

        public TableMetadata<DbType> SettingTableMetadata { get; }

        public bool CanRecodeData { get; set; } = true;

        public bool CanRecodeSetting { get; set; } = true;

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

        public override IEnumerable<Setting> ReadSettings(Setting setting)
        {
            var commandFactory = new SettingCommandFactory(SettingTableMetadata);

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
            var commandFactory = new SettingCommandFactory(SettingTableMetadata);

            using (var connection = new SQLiteConnection(ConnectionString))
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
                                if (CanRecodeSetting && setting.Value is string)
                                {
                                    setting.Value = ((string)setting.Value).Recode(SettingEncoding, DataEncoding);
                                }

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
