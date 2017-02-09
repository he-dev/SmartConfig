using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SqlServer
{
    internal class SettingCommandFactory
    {
        public SettingCommandFactory(TableMetadata<SqlDbType> tableMetadata)
        {
            TableMetadata = tableMetadata;
        }

        public TableMetadata<SqlDbType> TableMetadata { get; }

        public SqlCommand CreateSelectCommand(SqlConnection connection, Setting setting)
        {
            var sql = new StringBuilder();

            var dbProviderFactory = DbProviderFactories.GetFactory(connection);
            using (var commandBuilder = dbProviderFactory.CreateCommandBuilder())
            {
                var quote = new Func<string, string>(identifier => commandBuilder.QuoteIdentifier(identifier));

                var table = $"{quote(TableMetadata.SchemaName)}.{quote(TableMetadata.TableName)}";

                sql.Append($"SELECT *").AppendLine();
                sql.Append($"FROM {table}").AppendLine();
                sql.Append(setting.Tags.Aggregate(
                    $"WHERE ([{nameof(Setting.Name)}] = @{nameof(Setting.Name)} OR [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} + N'[[]%]')",
                    (result, next) => $"{result} AND {quote(next.Key)} = @{next.Key}")
                );
            }

            var command = connection.CreateCommand();
            command.CommandText = sql.ToString();

            // --- add parameters & values

            AddParameter(command, nameof(Setting.Name), setting.Name.WeakFullName);
            AddParameters(command, setting.Tags);

            return command;
        }

        public SqlCommand CreateDeleteCommand(SqlConnection connection, Setting setting)
        {
            /*
             
            DELETE FROM [dbo].[Setting] WHERE [Name] LIKE 'baz%' AND [Environment] = 'boz'

            */

            var sql = new StringBuilder();

            var dbProviderFactory = DbProviderFactories.GetFactory(connection);
            using (var commandBuilder = dbProviderFactory.CreateCommandBuilder())
            {
                var quote = new Func<string, string>(identifier => commandBuilder.QuoteIdentifier(identifier));

                var table = $"{quote(TableMetadata.SchemaName)}.{quote(TableMetadata.TableName)}";

                sql.Append($"DELETE FROM {table}").AppendLine();
                sql.Append(setting.Tags.Keys.Aggregate(
                    $"WHERE ([{nameof(Setting.Name)}] = @{nameof(Setting.Name)} OR [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} + N'[[]%]')",
                    (result, next) => $"{result} AND {quote(next)} = @{next} ")
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

        public SqlCommand CreateInsertCommand(SqlConnection connection, Setting setting)
        {
            /*
             
            UPDATE [Setting]
	            SET [Value] = 'Hallo update!'
	            WHERE [Name]='baz' AND [Environment] = 'boz'
            IF @@ROWCOUNT = 0 
	            INSERT INTO [Setting]([Name], [Value], [Environment])
	            VALUES ('baz', 'Hallo insert!', 'boz')
            
            */

            var sql = new StringBuilder();

            var dbProviderFactory = DbProviderFactories.GetFactory(connection);
            using (var commandBuilder = dbProviderFactory.CreateCommandBuilder())
            {
                var quote = new Func<string, string>(identifier => commandBuilder.QuoteIdentifier(identifier));

                var table = $"{quote(TableMetadata.SchemaName)}.{quote(TableMetadata.TableName)}";

                sql.Append($"UPDATE {table}").AppendLine();
                sql.Append($"SET [{nameof(Setting.Value)}] = @{nameof(Setting.Value)}").AppendLine();

                sql.Append(setting.Tags.Keys.Aggregate(
                    $"WHERE ([{nameof(Setting.Name)}] = @{nameof(Setting.Name)} OR [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} + N'[[]%]')",
                    (result, next) => $"{result} AND {quote(next)} = @{next} ")
                ).AppendLine();

                sql.Append($"IF @@ROWCOUNT = 0").AppendLine();

                var columns = setting.Tags.Keys.Select(columnName => quote(columnName)).Aggregate(
                        $"[{nameof(Setting.Name)}], [{nameof(Setting.Value)}]",
                        (result, next) => $"{result}, {next}"
                );

                sql.Append($"INSERT INTO {table}({columns})").AppendLine();

                var parameterNames = setting.Tags.Keys.Aggregate(
                    $"@{nameof(Setting.Name)}, @{nameof(Setting.Value)}",
                    (result, next) => $"{result}, @{next}"
                );

                sql.Append($"VALUES ({parameterNames})");
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

        private void AddParameter(SqlCommand command, string name, object value = null)
        {
            if (!TableMetadata.Columns.TryGetValue(name, out ColumnMetadata<SqlDbType> column))
            {
                throw new ColumnConfigurationNotFoundException(name);
            }

            var parameter = command.Parameters.Add(name, column.DbType, column.Length);

            if (value != null)
            {
                parameter.Value = value;
            }
        }

        private void AddParameters(SqlCommand command, IEnumerable<KeyValuePair<string, object>> parameters)
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

        public override string Message => $"\"{Column}\" column configuration not found. Ensure that it is set via the \"{nameof(SqlServerStore)}\" builder.";
    }
}
