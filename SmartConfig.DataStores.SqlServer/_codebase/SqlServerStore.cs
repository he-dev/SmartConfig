using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartUtilities;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig.DataStores.SqlServer
{
    /// <summary>
    /// Implements sql server data source.
    /// </summary>
    public class SqlServerStore : IDataStore
    {
        public SqlServerStore(string nameOrConnectionString, Action<SettingTableProperties.Builder> buildSettingTableProperties = null)
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

        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(Setting setting)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var commandFactory = new CommandFactory(SettingTableProperties);
                using (var command = commandFactory.CreateSelectCommand(connection, setting))
                {
                    connection.Open();
                    command.Prepare();

                    // execute query
                    using (var settingReader = command.ExecuteReader())
                    {
                        var settings = new List<Setting>();
                        while (settingReader.Read())
                        {
                            var result = new Setting
                            {
                                Name = new SettingPath((string)settingReader[nameof(Setting.Name)]),
                                Value = settingReader[nameof(Setting.Value)]
                            };
                            foreach (var property in setting.Namespaces)
                            {
                                result[property.Key] = settingReader[property.Key];
                            }
                            settings.Add(result);
                        }
                        return settings;
                    }
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

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var commandFactory = new CommandFactory(SettingTableProperties);

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var rowsAffected = 0;

                        // delete old settings
                        var firstSetting = settings.First();

                        using (var deleteCommand = commandFactory.CreateDeleteCommand(connection, firstSetting))
                        {
                            deleteCommand.Transaction = transaction;
                            deleteCommand.Prepare();

                            rowsAffected += deleteCommand.ExecuteNonQuery();
                        }

                        // insert new settings
                        using (var insertCommand = commandFactory.CreateInsertCommand(connection, firstSetting))
                        {
                            insertCommand.Transaction = transaction;
                            insertCommand.Prepare();

                            foreach (var setting in settings)
                            {
                                insertCommand.Parameters[nameof(Setting.Name)].Value = setting.Name.FullNameEx;
                                insertCommand.Parameters[nameof(Setting.Value)].Value = setting.Value;

                                foreach (var ns in setting.Namespaces)
                                {
                                    insertCommand.Parameters[ns.Key].Value = ns.Value;
                                }

                                rowsAffected += insertCommand.ExecuteNonQuery();
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

    public class SettingTableProperties
    {
        private static readonly SqlDbType DefaultSqlDbType = SqlDbType.NVarChar;

        private static readonly int DefaultColumnLength = 50;

        private readonly Dictionary<string, SqlDbType> _sqlDbTypes = new Dictionary<string, SqlDbType>();

        private readonly Dictionary<string, int> _columnLengths = new Dictionary<string, int>();

        public static readonly SettingTableProperties Default = new Builder().ToSettingTableProperties();

        private SettingTableProperties() { }

        public string SchemaName { get; private set; }

        public string TableName { get; private set; }

        public IReadOnlyDictionary<string, SqlDbType> SqlDbTypes => _sqlDbTypes;

        public IReadOnlyDictionary<string, int> ColumnLengths => _columnLengths;

        public SqlDbType GetSqlDbTypeOrDefault(string name)
        {
            SqlDbType result;
            return SqlDbTypes.TryGetValue(name, out result) ? result : DefaultSqlDbType;
        }

        public int GetColumnLengthOrDefault(string name)
        {
            int result;
            return ColumnLengths.TryGetValue(name, out result) ? result : DefaultColumnLength;
        }

        public Type MapDataType(Type objectType) => typeof(string);

        //public static SettingTableProperties Build(Action<Builder> build)
        //{
        //    var builder = new Builder();
        //    build(builder);
        //    return builder.ToSettingTableProperties();
        //}

        public class Builder
        {
            private SettingTableProperties _properties = new SettingTableProperties();

            private const int NVarCharMax = -1;

            internal Builder()
            {
                SchemaName("dbo");
                TableName("Setting");
                ColumnProperties("Name", SqlDbType.NVarChar, 200);
                ColumnProperties("Value", SqlDbType.NVarChar, NVarCharMax);
            }

            public Builder SchemaName(string schemaName)
            {
                _properties.SchemaName = schemaName;
                return this;
            }

            public Builder TableName(string tableName)
            {
                _properties.TableName = tableName;
                return this;
            }

            public Builder ColumnProperties(string columnName, SqlDbType sqlDbType, int length)
            {
                _properties._sqlDbTypes[columnName] = sqlDbType;
                _properties._columnLengths[columnName] = length;
                return this;
            }

            public Builder ColumnProperties(string columnName, SqlDbType sqlDbType)
            {
                _properties._sqlDbTypes[columnName] = sqlDbType;
                return this;
            }

            internal SettingTableProperties ToSettingTableProperties()
            {
                var result = _properties;
                _properties = null;
                return result;
            }
        }
    }
}

