using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements sql server data source.
    /// </summary>
    public class DbSource<TSetting> : DataSource<TSetting> where TSetting : Setting, new()
    {
        //public DbSource()
        //{
        //    using (var context = new SmartConfigContext<TSetting>(ConnectionString) { SettingsTableName = ""})
        //    {
        //        var objectContext = ((IObjectContextAdapter)context).ObjectContext;
        //        var objectSet = objectContext.CreateObjectSet<TSetting>();
        //        var keyNames = objectSet.EntitySet.ElementType.KeyNames.Select(k => k.Name).ToList();
        //    }
        //}

        /// <summary>
        /// Gets or sets the connection string where the config table can be found.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the config table name.
        /// </summary>
        public string SettingsTableName { get; set; }       

        public override string Select(string defaultKeyValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(defaultKeyValue));

            using (var context = CreateDbContext())
            {
                var compositeKey = CreateCompositeKey(defaultKeyValue);
                var name = compositeKey[KeyNames.DefaultKeyName];
                var elements = context.Settings.Where(ce => ce.Name == name).ToList() as IEnumerable<TSetting>;
                elements = ApplyFilters(elements, compositeKey);

                var element = elements.SingleOrDefault();
                return element == null ? null : element.Value;
            };
        }

        public override void Update(string defaultKeyValue, string value)
        {
            Debug.Assert(!string.IsNullOrEmpty(defaultKeyValue));

            using (var context = CreateDbContext())
            {
                var compositeKey = CreateCompositeKey(defaultKeyValue);

                // get key values
                var keyValues = compositeKey.Values.Cast<object>().ToArray();

                // find entity to update
                var entity = context.Settings.Find(keyValues);

                // there is no such entity yet so create a new one
                if (entity == null)
                {
                    // create a new entity
                    entity = new TSetting()
                    {
                        Name = compositeKey[KeyNames.DefaultKeyName],
                        Value = value
                    };

                    // set customKeys
                    foreach (var keyName in KeyNamesWithoutDefault)
                    {
                        entity[keyName] = compositeKey[keyName];
                    }

                    context.Settings.Add(entity);
                }
                // there is already such entity so just update the value
                else
                {
                    entity.Value = value;
                }

                context.SaveChanges();
            };
        }

        private SmartConfigContext<TSetting> CreateDbContext()
        {
            if (string.IsNullOrEmpty(ConnectionString)) throw new InvalidOperationException("ConnectionString must not be empty.");
            if (string.IsNullOrEmpty(SettingsTableName)) throw new InvalidOperationException("SettingsTableName must not be empty.");

            return new SmartConfigContext<TSetting>(ConnectionString)
            {
                SettingsTableName = SettingsTableName,
                SettingsTableKeyNames = KeyNames
            };
        }       
    }
}
