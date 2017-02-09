using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SQLite
{
    internal class SettingCommandFactory
    {
        public SettingCommandFactory(TableMetadata<DbType> tableMetadata)
        {
            TableMetadata = tableMetadata;
        }

        public TableMetadata<DbType> TableMetadata { get; }

        public SQLiteCommand CreateSelectCommand(SQLiteConnection connection, Setting setting)
        {
            // --- build sql

            // SELECT * FROM {table} WHERE [Name] = '{name}' AND 'Foo' = 'bar'

            var sql = new StringBuilder();

            var dbProviderFactory = DbProviderFactories.GetFactory(connection);
            using (var commandBuilder = dbProviderFactory.CreateCommandBuilder())
            {
                var quote = new Func<string, string>(identifier => commandBuilder.QuoteIdentifier(identifier));

                var table = $"{quote(TableMetadata.TableName)}";

                sql.Append($"SELECT * FROM {table}").AppendLine();
                sql.Append(setting.Tags.Keys.Aggregate(
                    $"WHERE ([{nameof(Setting.Name)}] = @{nameof(Setting.Name)} OR [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} || '[%]')",
                    (result, key) => $"{result} AND {quote(key)} = @{key}")
                );
            }

            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql.ToString();

            // --- add parameters

            AddParameter(command, nameof(Setting.Name), setting.Name.WeakFullName);
            AddParameters(command, setting.Tags);

            return command;
        }

        public SQLiteCommand CreateDeleteCommand(SQLiteConnection connection, Setting setting)
        {
            /*
             
            DELETE FROM [dbo].[Setting] WHERE [Name] LIKE 'baz%' AND [Environment] = 'boz'

            */

            var sql = new StringBuilder();

            var dbProviderFactory = DbProviderFactories.GetFactory(connection);
            using (var commandBuilder = dbProviderFactory.CreateCommandBuilder())
            {
                var quote = new Func<string, string>(identifier => commandBuilder.QuoteIdentifier(identifier));

                var table = $"{quote(TableMetadata.TableName)}";

                sql.Append($"DELETE FROM {table}").AppendLine();
                sql.Append(setting.Tags.Keys.Aggregate(
                    $"WHERE ([{nameof(Setting.Name)}] = @{nameof(Setting.Name)} OR [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} || '[%]')",
                    (result, key) => $"{result} AND {quote(key)} = @{key} ")
                );
            }

            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql.ToString();

            // --- add parameters & values

            AddParameter(command, nameof(Setting.Name), setting.Name.WeakFullName);
            AddParameters(command, setting.Tags);

            return command;
        }

        public SQLiteCommand CreateInsertCommand(SQLiteConnection connection, Setting setting)
        {
            /*
                INSERT OR REPLACE INTO Setting([Name], [Value])
                VALUES('{setting.Name.FullNameEx}', '{setting.Value}')
            */

            // --- build sql

            var sql = new StringBuilder();

            var dbProviderFactory = DbProviderFactories.GetFactory(connection);
            using (var commandBuilder = dbProviderFactory.CreateCommandBuilder())
            {
                var quote = new Func<string, string>(identifier => commandBuilder.QuoteIdentifier(identifier));

                var table = $"{quote(TableMetadata.TableName)}";

                var columns = setting.Tags.Keys.Select(columnName => quote(columnName)).Aggregate(
                    $"[{nameof(Setting.Name)}], [{nameof(Setting.Value)}]",
                    (result, next) => $"{result}, {next}"
                );

                sql.Append($"INSERT OR REPLACE INTO {table}({columns})").AppendLine();

                var parameterNames = setting.Tags.Keys.Aggregate(
                        $"@{nameof(Setting.Name)}, @{nameof(Setting.Value)}",
                        (result, next) => $"{result}, @{next}"
                );

                sql.Append($"VALUES ({parameterNames})").AppendLine();
            }

            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql.ToString();

            // --- add parameters

            AddParameter(command, nameof(Setting.Name), setting.Name.StrongFullName);
            AddParameter(command, nameof(Setting.Value), setting.Value);
            AddParameters(command, setting.Tags);

            return command;
        }

        private void AddParameter(SQLiteCommand command, string name, object value = null)
        {
            if (!TableMetadata.Columns.TryGetValue(name, out ColumnMetadata<DbType> column))
            {
                throw new ColumnConfigurationNotFoundException(name);
            }

            var parameter = command.Parameters.Add(name, column.DbType, column.Length);

            if (value != null)
            {
                parameter.Value = value;
            }
        }

        private void AddParameters(SQLiteCommand command, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            foreach (var parameter in parameters)
            {
                AddParameter(command, parameter.Key, parameter.Value);
            }
        }
    }

    public class ColumnConfigurationNotFoundException : Exception
    {
        internal ColumnConfigurationNotFoundException(string column)
        {
            Column = column;
        }

        public string Column { get; set; }

        public override string Message => $"\"{Column}\" column configuration not found. Ensure that it is set via the \"{nameof(SQLiteStore)}\" builder.";
    }
}
