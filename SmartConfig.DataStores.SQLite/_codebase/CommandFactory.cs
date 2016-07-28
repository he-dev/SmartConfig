using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SQLite
{
    internal class CommandFactory
    {
        public CommandFactory(SettingTableProperties settingTableProperties)
        {
            SettingTableProperties = settingTableProperties;
        }

        public SettingTableProperties SettingTableProperties { get; }

        public SQLiteCommand CreateSelectCommand(SQLiteConnection connection, Setting setting)
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

                var where = setting.Namespaces.Aggregate(
                    $"WHERE ([{nameof(Setting.Name)}] = @{nameof(Setting.Name)} OR [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} || '[%]')",
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
                ).Value = setting.Name.FullName;

            foreach (var ns in setting.Namespaces)
            {
                command.Parameters.Add(
                    ns.Key,
                    SettingTableProperties.GetDbTypeOrDefault(ns.Key),
                    SettingTableProperties.GetColumnLengthOrDefault(ns.Key)
                    ).Value = ns.Value;
            }

            return command;
        }

        //public SQLiteCommand CreateDeleteCommand(SQLiteConnection connection, Setting setting)
        //{
        //    /*
             
        //    DELETE FROM [dbo].[Setting] WHERE [Name] LIKE 'baz%' AND [Environment] = 'boz'

        //    */

        //    var sql = new StringBuilder();

        //    var dbProviderFactory = DbProviderFactories.GetFactory(connection);
        //    using (var commandBuilder = dbProviderFactory.CreateCommandBuilder())
        //    {
        //        //var schemaName = commandBuilder.QuoteIdentifier(SettingTableProperties.SchemaName);
        //        var tableName = commandBuilder.QuoteIdentifier(SettingTableProperties.TableName);

        //        sql.Append($"DELETE FROM {tableName}").AppendLine();
        //        sql.Append(setting.Namespaces.Keys.Aggregate(
        //            //$"WHERE [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} + N'%'",
        //            $"WHERE ([{nameof(Setting.Name)}] = @{nameof(Setting.Name)} OR [{nameof(Setting.Name)}] LIKE @{nameof(Setting.Name)} + N'[[]%]')",
        //            (result, next) => $"{result} AND {commandBuilder.QuoteIdentifier(next)} = @{next} ")
        //        );
        //    }

        //    var command = connection.CreateCommand();
        //    command.CommandType = CommandType.Text;
        //    command.CommandText = sql.ToString();

        //    // --- add parameters & values

        //    command.Parameters.Add(
        //        nameof(Setting.Name),
        //        SettingTableProperties.GetSqlDbTypeOrDefault(nameof(Setting.Name)),
        //        SettingTableProperties.GetColumnLengthOrDefault(nameof(Setting.Name))
        //    ).Value = setting.Name.FullName;

        //    foreach (var ns in setting.Namespaces)
        //    {
        //        command.Parameters.Add(
        //            ns.Key,
        //            SettingTableProperties.GetSqlDbTypeOrDefault(ns.Key),
        //            SettingTableProperties.GetColumnLengthOrDefault(ns.Key)
        //        ).Value = ns.Value;
        //    }

        //    return command;
        //}

        public SQLiteCommand CreateInsertCommand(SQLiteConnection connection, Setting setting)
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

                var quotedColumnNames = setting.Namespaces.Keys
                        .Select(columnName => commandBuilder.QuoteIdentifier(columnName))
                        .Aggregate(
                            $"[{nameof(Setting.Name)}], [{nameof(Setting.Value)}]", 
                            (result, next) => $"{result}, {next}");

                sql.Append($"INSERT OR REPLACE INTO {tableName}({quotedColumnNames})").AppendLine();

                var parameterNames = setting.Namespaces.Keys.Aggregate(
                        $"@{nameof(Setting.Name)}, @{nameof(Setting.Value)}", 
                        (result, next) => $"{result}, @{next}");

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

            foreach (var ns in setting.Namespaces)
            {
                command.Parameters.Add(
                    ns.Key,
                    SettingTableProperties.GetDbTypeOrDefault(ns.Key),
                    SettingTableProperties.GetColumnLengthOrDefault(ns.Key)
                );
            }

            return command;
        }
    }
}
