using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    public class SQLiteStore : IDataStore
    {
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

        public bool UTF8FixEnabled { get; set; } = true;

        public Type MapDataType(Type settingType) => typeof(string);

        public List<Setting> GetSettings(SettingPath path, IReadOnlyDictionary<string, object> namespaces)
        {
            path.Validate(nameof(path)).IsNotNull();
            namespaces.Validate(nameof(namespaces)).IsNotNull();

            using (var connection = new SQLiteConnection(ConnectionString)) // "Data Source=config.db;Version=3;"))
            using (var command = CreateSelectCommand(connection, path, namespaces))
            {
                connection.Open();
                command.Prepare();

                using (var settingReader = command.ExecuteReader())
                {
                    var settings = new List<Setting>();

                    while (settingReader.Read())
                    {
                        var value = (string)settingReader[nameof(Setting.Value)];

                        var setting = new Setting
                        {
                            Name = (string)settingReader[nameof(Setting.Name)],
                            Value = UTF8FixEnabled ? value.AsUTF8() : value
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

        public int SaveSetting(SettingPath path, IReadOnlyDictionary<string, object> namespaces, object value)
        {
            throw new NotImplementedException();
        }

        public int SaveSettings(IReadOnlyDictionary<SettingPath, object> settings, IReadOnlyDictionary<string, object> namespaces)
        {
            using (var connection = new SQLiteConnection(ConnectionString)) // "Data Source=config.db;Version=3;"))
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

        private SQLiteCommand CreateSelectCommand(SQLiteConnection connection, SettingPath name, IReadOnlyDictionary<string, object> namespaces)
        {
            // --- build sql

            // SELECT * FROM {table} WHERE [Name] = '{name}' AND 'Foo' = 'bar'

            var sql = new StringBuilder();

            var dbProviderFactory = DbProviderFactories.GetFactory(connection);
            using (var commandBuilder = dbProviderFactory.CreateCommandBuilder())
            {
                var tableName = commandBuilder.QuoteIdentifier(SettingTableProperties.TableName);

                sql.Append($"SELECT *").AppendLine();
                sql.Append($"FROM {tableName}").AppendLine();

                var where = namespaces.Aggregate(
                    $"WHERE [{nameof(Setting.Name)}] = @{nameof(Setting.Name)}",
                    (result, next) => $"{result} AND {commandBuilder.QuoteIdentifier(next.Key)} = @{next.Key}");
                sql.Append(where);
            }

            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql.ToString();

            // --- add parameters

            command.Parameters.Add(
                nameof(Setting.Name),
                SettingTableProperties.GetDbTypeOrDefault(nameof(Setting.Name)),
                SettingTableProperties.GetColumnLengthOrDefault(nameof(Setting.Name))
                ).Value = name.ToString();

            foreach (var settingNamespace in namespaces)
            {
                command.Parameters.Add(
                    settingNamespace.Key,
                    SettingTableProperties.GetDbTypeOrDefault(settingNamespace.Key),
                    SettingTableProperties.GetColumnLengthOrDefault(settingNamespace.Key)
                    ).Value = settingNamespace.Value;
            }

            return command;
        }

        private SQLiteCommand CreateInsertCommand(SQLiteConnection connection, IReadOnlyDictionary<string, object> namespaces)
        {
            /*
                INSERT OR REPLACE INTO Setting([Name], [Value])
                VALUES('{key.Main.Value}', '{value}')
            */

            // --- build sql

            var sql = new StringBuilder();

            var dbProviderFactory = DbProviderFactories.GetFactory(connection);
            using (var commandBuilder = dbProviderFactory.CreateCommandBuilder())
            {
                var tableName = commandBuilder.QuoteIdentifier(SettingTableProperties.TableName);

                var quotedColumnNames = namespaces.Keys
                        .Select(columnName => commandBuilder.QuoteIdentifier(columnName))
                        .Aggregate($"[{nameof(Setting.Name)}], [{nameof(Setting.Value)}]", (result, next) => $"{result}, {next}");

                sql.Append($"INSERT OR REPLACE INTO {tableName}({quotedColumnNames})").AppendLine();

                var parameterNames = namespaces.Keys
                    .Aggregate($"@{nameof(Setting.Name)}, @{nameof(Setting.Value)}", (result, next) => $"{result}, @{next}");
                sql.Append($"VALUES ({parameterNames})").AppendLine();
            }

            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql.ToString();

            // --- add parameters

            command.Parameters.Add(
                nameof(Setting.Name),
                SettingTableProperties.GetDbTypeOrDefault(nameof(Setting.Name)),
                SettingTableProperties.GetColumnLengthOrDefault(nameof(Setting.Name))
            );

            command.Parameters.Add(
                nameof(Setting.Value),
                SettingTableProperties.GetDbTypeOrDefault(nameof(Setting.Value)),
                SettingTableProperties.GetColumnLengthOrDefault(nameof(Setting.Value))
            );

            foreach (var settingNamespace in namespaces)
            {
                command.Parameters.Add(
                    settingNamespace.Key,
                    SettingTableProperties.GetDbTypeOrDefault(settingNamespace.Key),
                    SettingTableProperties.GetColumnLengthOrDefault(settingNamespace.Key)
                );
            }

            return command;
        }
    }

    public class SettingTableProperties
    {
        public string TableName { get; private set; }

        private static readonly DbType DefaultDbType = DbType.String;

        private static readonly int DefaultColumnLength = 50;

        private readonly Dictionary<string, DbType> _dbTypes = new Dictionary<string, DbType>();

        private readonly Dictionary<string, int> _columnLengths = new Dictionary<string, int>();

        public IReadOnlyDictionary<string, DbType> DbTypes => _dbTypes;

        public IReadOnlyDictionary<string, int> ColumnLengths => _columnLengths;

        public DbType GetDbTypeOrDefault(string name)
        {
            DbType result;
            return DbTypes.TryGetValue(name, out result) ? result : DefaultDbType;
        }

        public int GetColumnLengthOrDefault(string name)
        {
            int result;
            return ColumnLengths.TryGetValue(name, out result) ? result : DefaultColumnLength;
        }

        public class Builder
        {
            private SettingTableProperties _properties = new SettingTableProperties();

            internal Builder()
            {
                TableName("Setting");
                ColumnProperties("Name", DbType.String, 200);
                ColumnProperties("Value", DbType.String, -1);
            }

            public Builder TableName(string tableName)
            {
                _properties.TableName = tableName;
                return this;
            }

            public Builder ColumnProperties(string columnName, DbType dbType, int length)
            {
                _properties._dbTypes[columnName] = dbType;
                _properties._columnLengths[columnName] = length;
                return this;
            }

            public Builder ColumnProperties(string columnName, DbType dbType)
            {
                _properties._dbTypes[columnName] = dbType;
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
