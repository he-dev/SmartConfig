using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using SmartConfig.Collections;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements sql server data source.
    /// </summary>
    public class DbSource<TSetting> : DataSource<TSetting> where TSetting : Setting, new()
    {
        private readonly string _connectionString;
        private readonly string _settingsTableName;

        public DbSource(string connectionString, string settingsTableName)
        {
            if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentNullException(nameof(connectionString)); }
            if (string.IsNullOrEmpty(settingsTableName)) { throw new ArgumentNullException(nameof(settingsTableName)); }

            var isConnectionStringName = connectionString.StartsWith("name=", StringComparison.OrdinalIgnoreCase);
            _connectionString =
                isConnectionStringName
                ? ConfigurationManager.ConnectionStrings[Regex.Replace(connectionString, "^name=", string.Empty, RegexOptions.IgnoreCase)].ConnectionString
                : connectionString;

            if (string.IsNullOrEmpty(_connectionString)) { throw new ArgumentNullException(nameof(connectionString)); }

            _settingsTableName = settingsTableName;
        }

        public override IReadOnlyCollection<Type> SupportedTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKeyCollection keys)
        {
            Debug.Assert(keys != null);

            using (var context = new SmartConfigContext<TSetting>(_connectionString, _settingsTableName))
            {
                var name = keys.DefaultKey.Value;
                var settings = context.Settings.Where(ce => ce.Name == name).ToList() as IEnumerable<TSetting>;

                settings = ApplyFilters(settings, keys.Skip(1));

                var setting = settings.SingleOrDefault();
                return setting?.Value;
            }
        }

        public override void Update(SettingKeyCollection keys, object value)
        {
            Debug.Assert(keys != null && keys.Any());

            using (var context = new SmartConfigContext<TSetting>(_connectionString, _settingsTableName))
            {
                var keyValues = keys.Select(x => x.Value).Cast<object>().ToArray();
                var entity = context.Settings.Find(keyValues);

                var entityExists = entity != null;
                if (!entityExists)
                {
                    entity = new TSetting()
                    {
                        Name = (SettingPath)keys.DefaultKey.Value,
                        Value = value?.ToString()
                    };

                    // set customKeys
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
