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

        public override IReadOnlyCollection<Type> SupportedSettingDataTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(CompoundSettingKey key)
        {
            using (var conn = new SQLiteConnection(_connectionString)) // "Data Source=config.db;Version=3;"))
            {
                conn.Open();

                var sql = "SELECT * FROM {table} WHERE Name = '{name}'".Format(new
                {
                    table = _settingsTableName,
                    name = key.NameKey.Value.ToString()
                });

                using (var cmd = new SQLiteCommand(sql, conn))
                using (var settingReader = cmd.ExecuteReader())
                {
                    var settings = new List<TSetting>();

                    while (settingReader.Read())
                    {
                        var setting = new TSetting
                        {
                            Name = (string)settingReader["Name"],
                            Value = (string)settingReader["Value"]
                        };

                        var settingKeyNames = SettingKeyNameCollection.Create<TSetting>();
                        foreach (var customKeyName in settingKeyNames.CustomKeyNames)
                        {
                            ((IIndexable)setting)[customKeyName] = (string)settingReader[customKeyName];
                        }
                        settings.Add(setting);
                    }

                    settings = ApplyFilters(settings, key.CustomKeys).ToList();
                    var result = settings.FirstOrDefault();
                    return result?.Value;
                }
            }
        }

        public override void Update(CompoundSettingKey key, object value)
        {
            // INSERT OR REPLACE INTO Setting(Name, Value, Environment) VALUES('Greeting' 'Hallo SQLite!', 'sqlite',);


            using (var conn = new SQLiteConnection(_connectionString)) // "Data Source=config.db;Version=3;"))
            {
                conn.Open();

                var otherKeys = string.Join(" ", key.CustomKeys.Select(x => $", {x.Name}"));
                var otherValues = string.Join(" ", key.CustomKeys.Select(x => $", '{x.Value.ToString()}'"));

                var sql =
                    $"INSERT OR REPLACE INTO Setting(Name, Value{otherKeys}) " +
                    $"VALUES('{key.NameKey.Value}', '{value}'{otherValues})";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    var affectedRows = cmd.ExecuteNonQuery();
                    Debug.Assert(affectedRows == 1, "Invalid number of affected rows.");
                }
            }
        }
    }
}
