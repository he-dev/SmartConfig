using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartUtilities.Collections;

namespace SmartConfig.DataStores.SqlServer
{
    /// <summary>
    /// Implements sql server data source.
    /// </summary>
    public class SqlServerStore<TSetting> : DataStore<TSetting> where TSetting : BasicSetting, new()
    {
        private readonly string _connectionString;
        private readonly string _settingsTableName;

        public SqlServerStore(string connectionString, string settingsTableName)
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
                throw new ConnectionStringNotFoundException
                {
                    ConnectionStringName = connectionStringNameMatch.Groups["connectionStringName"].Value
                };
            }
        }

        public override IReadOnlyCollection<Type> SupportedSettingDataTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(CompoundSettingKey keys)
        {
            Debug.Assert(keys != null);

            using (var context = new SqlServerContext<TSetting>(_connectionString, _settingsTableName))
            {
                var settings = context.Settings.Where(ce => ce.Name == keys.NameKey).ToList() as IEnumerable<TSetting>;

                settings = ApplyFilters(settings, keys.Skip(1));

                var setting = settings.FirstOrDefault();
                return setting?.Value;
            }
        }

        public override void Update(CompoundSettingKey keys, object value)
        {
            Debug.Assert(keys != null && keys.Any());

            using (var context = new SqlServerContext<TSetting>(_connectionString, _settingsTableName))
            {
                var keyValues = keys.Select(x => (object)x.Value.ToString()).ToArray();
                var entity = context.Settings.Find(keyValues);

                var entityExists = entity != null;
                if (!entityExists)
                {
                    entity = new TSetting
                    {
                        Name = keys.NameKey.Value.ToString(),
                        Value = value?.ToString()
                    };

                    // set custom keys
                    foreach (var key in keys.CustomKeys)
                    {
                        entity[key.Name] = key.Value.ToString();
                    }

                    context.Settings.Add(entity);
                }
                else
                {
                    entity.Value = value?.ToString();
                }

                context.SaveChanges();
            }
        }
    }
}
