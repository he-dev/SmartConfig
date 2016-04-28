using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartUtilities;
using SmartUtilities.Collections;

namespace SmartConfig.DataStores.SQLite
{
    public class SQLiteStore<TSetting> : DataStore<TSetting> where TSetting : BasicSetting, new()
    {
        private readonly string _connectionString;
        private readonly string _settingsTableName;

        public bool UTF8FixEnabled { get; set; } = true;

        public SQLiteStore(string connectionString, string settingsTableName)
        {
            if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentNullException(nameof(connectionString)); }
            if (string.IsNullOrEmpty(settingsTableName)) { throw new ArgumentNullException(nameof(settingsTableName)); }

            _connectionString = connectionString;
            _settingsTableName = settingsTableName;


            var connectionStringNameMatch = Regex.Match(_connectionString, "^name=(?<connectionStringName>.+)", RegexOptions.IgnoreCase);
            if (connectionStringNameMatch.Success)
            {
                var connectionStringsRepository = new ConnectionStringsRepository();
                _connectionString = connectionStringsRepository[connectionStringNameMatch.Groups["connectionStringName"].Value];
            }

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exceptions.ConnectionStringNotFoundException
                {
                    ConnectionStringName = connectionStringNameMatch.Groups["connectionStringName"].Value
                };
            }
        }

        public override IReadOnlyCollection<Type> SerializationTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKey key)
        {
            using (var conn = new SQLiteConnection(_connectionString)) // "Data Source=config.db;Version=3;"))
            {
                conn.Open();

                var sql = "SELECT * FROM {table} WHERE [Name] = '{name}'".Format(new
                {
                    table = _settingsTableName,
                    name = key.Main.Value.ToString()
                });

                using (var cmd = new SQLiteCommand(sql, conn))
                using (var settingReader = cmd.ExecuteReader())
                {
                    var settings = new List<TSetting>();

                    while (settingReader.Read())
                    {
                        var value = (string)settingReader["Value"];

                        var setting = new TSetting
                        {
                            Name = (string)settingReader[key.Main.Key],
                            Value = UTF8FixEnabled ? value.AsUTF8() : value
                        };

                        foreach (var custom in key.CustomKeys)
                        {
                            ((IIndexable)setting)[custom.Key] = (string)settingReader[custom.Key];
                        }
                        settings.Add(setting);
                    }

                    settings = ApplyFilters(settings, key.CustomKeys).ToList();
                    var result = settings.FirstOrDefault();
                    return result?.Value;
                }
            }
        }

        public override void Update(SettingKey key, object value)
        {
            // INSERT OR REPLACE INTO Setting(Main, Value, Environment) VALUES('Greeting' 'Hallo SQLite!', 'sqlite',);


            using (var conn = new SQLiteConnection(_connectionString)) // "Data Source=config.db;Version=3;"))
            {
                conn.Open();

                var otherKeys = string.Join(" ", key.CustomKeys.Select(custom => $", [{custom.Key}]"));
                var otherValues = string.Join(" ", key.CustomKeys.Select(custom => $", '{custom.Value.ToString()}'"));

                var sql =
                    $"INSERT OR REPLACE INTO Setting([Name], [Value]{otherKeys}) " +
                    $"VALUES('{key.Main.Value}', '{value}'{otherValues})";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    var affectedRows = cmd.ExecuteNonQuery();
                    Debug.Assert(affectedRows == 1, "Invalid number of affected rows.");
                }
            }
        }
    }
}
