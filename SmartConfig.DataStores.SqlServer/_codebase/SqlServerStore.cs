using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
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

        public override IReadOnlyCollection<Type> SerializationTypes { get; } = new ReadOnlyCollection<Type>(new[] { typeof(string) });

        public override object Select(SettingKey key)
        {
            Debug.Assert(key != null && key.Any());

            using (var context = new SqlServerContext<TSetting>(_connectionString, _settingsTableName))
            {
                var name = key.Main.Value.ToString();
                var settings = context.Settings.Where(ce => ce.Name == name).ToList() as IEnumerable<TSetting>;

                settings = ApplyFilters(settings, key.CustomKeys);

                var setting = settings.FirstOrDefault();
                return setting?.Value;
            }
        }

        public override void Update(SettingKey key, object value)
        {
            Debug.Assert(key != null && key.Any());

            using (var context = new SqlServerContext<TSetting>(_connectionString, _settingsTableName))
            {
#if DEBUG
                // debug code
                //var createEntityKey = new Func<DbContext, TSetting, EntityKey>((c, e) =>
                //{
                //    const string entitySetName = "Settings";
                //    var entityKey = ((IObjectContextAdapter)c).ObjectContext.CreateEntityKey(entitySetName, e);
                //    return entityKey;
                //});
                //var entityKey3 = createEntityKey(context, context.Settings.FirstOrDefault());
                //context.Database.Log = sql => Debug.WriteLine(sql);
#endif

                var keyValues = key.Select(x => (object)x.Value.ToString()).ToArray();
                var entity = context.Settings.Find(keyValues);

                if (entity == null)
                {
                    entity = new TSetting
                    {
                        Name = key.Main.Value.ToString(),
                        Value = value?.ToString()
                    };

                    // set custom keys
                    foreach (var custom in key.CustomKeys)
                    {
                        entity[custom.Key] = custom.Value.ToString();
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
