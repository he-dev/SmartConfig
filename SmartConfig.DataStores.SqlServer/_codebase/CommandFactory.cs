using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SqlServer
{
    internal class CommandFactory
    {
        public CommandFactory(SettingTableProperties settingTableProperties)
        {
            SettingTableProperties = settingTableProperties;
        }

        public SettingTableProperties SettingTableProperties { get; }

        public SqlCommand CreateSelectCommand(SqlConnection connection, Setting setting)
        {
            var sql = new StringBuilder();

            var dbProviderFactory = DbProviderFactories.GetFactory(connection);
            using (var commandBuilder = dbProviderFactory.CreateCommandBuilder())
            {
                var schemaName = commandBuilder.QuoteIdentifier(SettingTableProperties.SchemaName);
                var tableName = commandBuilder.QuoteIdentifier(SettingTableProperties.TableName);

                sql.Append($"SELECT *").AppendLine();
                sql.Append($"FROM {schemaName}.{tableName}").AppendLine();
                sql.Append(setting.Namespaces.Aggregate(
                    $"WHERE ([{nameof(Setting.Name)}] = @{nameof(Setting.Name)} OR [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} + N'[[]%]')",
                    (result, next) => $"{result} AND {commandBuilder.QuoteIdentifier(next.Key)} = @{next.Key}")
                );
            }

            var command = connection.CreateCommand();
            command.CommandText = sql.ToString();

            // --- add parameters & values

            command.Parameters.Add(
                    nameof(Setting.Name),
                    SettingTableProperties.GetSqlDbTypeOrDefault(nameof(Setting.Name)),
                    SettingTableProperties.GetColumnLengthOrDefault(nameof(Setting.Name))
                ).Value = setting.Name.FullName;

            foreach (var ns in setting.Namespaces)
            {
                command.Parameters.Add(
                    ns.Key,
                    SettingTableProperties.GetSqlDbTypeOrDefault(ns.Key),
                    SettingTableProperties.GetColumnLengthOrDefault(ns.Key)
                ).Value = ns.Value;
            }

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
                var schemaName = commandBuilder.QuoteIdentifier(SettingTableProperties.SchemaName);
                var tableName = commandBuilder.QuoteIdentifier(SettingTableProperties.TableName);

                sql.Append($"DELETE FROM {schemaName}.{tableName}").AppendLine();
                sql.Append(setting.Namespaces.Keys.Aggregate(
                    //$"WHERE [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} + N'%'",
                    $"WHERE ([{nameof(Setting.Name)}] = @{nameof(Setting.Name)} OR [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} + N'[[]%]')",
                    (result, next) => $"{result} AND {commandBuilder.QuoteIdentifier(next)} = @{next} ")
                );
            }

            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql.ToString();

            // --- add parameters & values

            command.Parameters.Add(
                nameof(Setting.Name),
                SettingTableProperties.GetSqlDbTypeOrDefault(nameof(Setting.Name)),
                SettingTableProperties.GetColumnLengthOrDefault(nameof(Setting.Name))
            );//.Value = setting.Name.FullName;

            foreach (var ns in setting.Namespaces)
            {
                command.Parameters.Add(
                    ns.Key,
                    SettingTableProperties.GetSqlDbTypeOrDefault(ns.Key),
                    SettingTableProperties.GetColumnLengthOrDefault(ns.Key)
                );//.Value = ns.Value;
            }            

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
                var schemaName = commandBuilder.QuoteIdentifier(SettingTableProperties.SchemaName);
                var tableName = commandBuilder.QuoteIdentifier(SettingTableProperties.TableName);

                sql.Append($"UPDATE {schemaName}.{tableName}").AppendLine();
                sql.Append($"SET [{nameof(Setting.Value)}] = @{nameof(Setting.Value)}").AppendLine();

                sql.Append(setting.Namespaces.Keys.Aggregate(
                    //$"WHERE [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} + N'%'", 
                    $"WHERE ([{nameof(Setting.Name)}] = @{nameof(Setting.Name)} OR [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} + N'[[]%]')",
                    (result, next) => $"{result} AND {commandBuilder.QuoteIdentifier(next)} = @{next} ")
                ).AppendLine();

                sql.Append($"IF @@ROWCOUNT = 0").AppendLine();

                var quotedColumnNames = setting.Namespaces.Keys
                    .Select(columnName => commandBuilder.QuoteIdentifier(columnName)).Aggregate(
                        $"[{nameof(Setting.Name)}], [{nameof(Setting.Value)}]", 
                        (result, next) => $"{result}, {next}");

                sql.Append($"INSERT INTO {schemaName}.{tableName}({quotedColumnNames})").AppendLine();

                var parameterNames = setting.Namespaces.Keys.Aggregate(
                    $"@{nameof(Setting.Name)}, @{nameof(Setting.Value)}", 
                    (result, next) => $"{result}, @{next}");

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

            foreach (var ns in setting.Namespaces)
            {
                command.Parameters.Add(
                    ns.Key,
                    SettingTableProperties.GetSqlDbTypeOrDefault(ns.Key),
                    SettingTableProperties.GetColumnLengthOrDefault(ns.Key)
                );
            }

            return command;
        }

    }
}
