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

        public List<Setting> GetSettings(SettingPath name, IReadOnlyDictionary<string, object> namespaces)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = CreateSelectCommand(connection, name, namespaces))
            {
                connection.Open();
                command.Prepare();

                // execute query
                using (var settingReader = command.ExecuteReader())
                {
                    var settings = new List<Setting>();
                    while (settingReader.Read())
                    {
                        var setting = new Setting
                        {
                            Name = (string)settingReader[nameof(Setting.Name)],
                            Value = settingReader[nameof(Setting.Value)]
                        };
                        foreach (var property in namespaces)
                        {
                            setting[property.Key] = settingReader[property.Key];
                        }
                        settings.Add(setting);
                    }
                    return settings;
                }
            }
        }

        public int SaveSetting(SettingPath name, IReadOnlyDictionary<string, object> namespaces, object value)
        {
            return SaveSettings(new Dictionary<SettingPath, object> {[name] = value}, namespaces);
        }

        public int SaveSettings(IReadOnlyDictionary<SettingPath, object> settings, IReadOnlyDictionary<string, object> namespaces)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                using (var command = CreateInsertCommand(connection, namespaces))
                {
                    command.Transaction = transaction;
                    command.Prepare();

                    try
                    {
                        var affectedSettings = 0;
                        foreach (var setting in settings)
                        {
                            command.Parameters[nameof(Setting.Name)].Value = setting.Key.ToString();
                            command.Parameters[nameof(Setting.Value)].Value = setting.Value;

                            foreach (var settingNamespace in namespaces)
                            {
                                command.Parameters[settingNamespace.Key].Value = settingNamespace.Value;
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

        private SqlCommand CreateSelectCommand(SqlConnection connection, SettingPath name, IReadOnlyDictionary<string, object> namespaces)
        {
            // --- build sql

            var sql = new StringBuilder();

            var dbProviderFactory = DbProviderFactories.GetFactory(connection);
            using (var commandBuilder = dbProviderFactory.CreateCommandBuilder())
            {
                var schemaName = commandBuilder.QuoteIdentifier(SettingTableProperties.SchemaName);
                var tableName = commandBuilder.QuoteIdentifier(SettingTableProperties.TableName);

                sql.Append($"SELECT *").AppendLine();
                sql.Append($"FROM {schemaName}.{tableName}").AppendLine();

                var where = namespaces.Aggregate($"WHERE [{nameof(Setting.Name)}] = @{nameof(Setting.Name)}", (result, next) =>
                    $"{result} AND {commandBuilder.QuoteIdentifier(next.Key)} = @{next.Key}");
                sql.Append(where);
            }

            var command = connection.CreateCommand();
            command.CommandText = sql.ToString();

            // --- add parameters

            command.Parameters.Add(
                    nameof(Setting.Name),
                    SettingTableProperties.GetSqlDbTypeOrDefault(nameof(Setting.Name)),
                    SettingTableProperties.GetColumnLengthOrDefault(nameof(Setting.Name))
                ).Value = name.ToString();

            foreach (var settingNamespace in namespaces)
            {
                command.Parameters.Add(
                    settingNamespace.Key,
                    SettingTableProperties.GetSqlDbTypeOrDefault(settingNamespace.Key),
                    SettingTableProperties.GetColumnLengthOrDefault(settingNamespace.Key)
                ).Value = settingNamespace.Value;
            }

            return command;
        }

        private SqlCommand CreateInsertCommand(SqlConnection connection, IReadOnlyDictionary<string, object> namespaces)
        {
            /*
             
            UPDATE [Setting]
	            SET [Value] = 'Hallo update!'
	            WHERE [Name]='baz' AND [Environment] = 'boz'
            IF @@ROWCOUNT = 0 
	            INSERT INTO [Setting]([Name], [Value], [Environment])
	            VALUES ('baz', 'Hallo insert!', 'boz')
             */

            // --- build sql

            var sql = new StringBuilder();

            var dbProviderFactory = DbProviderFactories.GetFactory(connection);
            using (var commandBuilder = dbProviderFactory.CreateCommandBuilder())
            {
                var schemaName = commandBuilder.QuoteIdentifier(SettingTableProperties.SchemaName);
                var tableName = commandBuilder.QuoteIdentifier(SettingTableProperties.TableName);

                sql.Append($"UPDATE {schemaName}.{tableName}").AppendLine();
                sql.Append($"SET [{nameof(Setting.Value)}] = @{nameof(Setting.Value)}").AppendLine();

                var where = namespaces.Keys
                    .Aggregate($"WHERE [{nameof(Setting.Name)}] = @{nameof(Setting.Name)}", (result, next) =>
                        $"{result} AND {commandBuilder.QuoteIdentifier(next)} = @{next} ");
                sql.Append(where).AppendLine();

                sql.Append($"IF @@ROWCOUNT = 0").AppendLine();

                var quotedColumnNames = namespaces.Keys
                        .Select(columnName => commandBuilder.QuoteIdentifier(columnName))
                        .Aggregate($"[{nameof(Setting.Name)}], [{nameof(Setting.Value)}]", (result, next) => $"{result}, {next}");

                sql.Append($"INSERT INTO {schemaName}.{tableName}({quotedColumnNames})").AppendLine();

                var parameterNames = namespaces.Keys
                    .Aggregate($"@{nameof(Setting.Name)}, @{nameof(Setting.Value)}", (result, next) => $"{result}, @{next}");
                sql.Append($"VALUES ({parameterNames})");
            }

            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql.ToString();

            // --- add parameters

            command.Parameters.Add(
                nameof(Setting.Name),
                SettingTableProperties.GetSqlDbTypeOrDefault(nameof(Setting.Name)),
                SettingTableProperties.GetColumnLengthOrDefault(nameof(Setting.Name))
            );

            command.Parameters.Add(
                nameof(Setting.Value),
                SettingTableProperties.GetSqlDbTypeOrDefault(nameof(Setting.Value)),
                SettingTableProperties.GetColumnLengthOrDefault(nameof(Setting.Value))
            );

            foreach (var settingNamespace in namespaces)
            {
                command.Parameters.Add(
                    settingNamespace.Key,
                    SettingTableProperties.GetSqlDbTypeOrDefault(settingNamespace.Key),
                    SettingTableProperties.GetColumnLengthOrDefault(settingNamespace.Key)
                );
            }

            return command;
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

