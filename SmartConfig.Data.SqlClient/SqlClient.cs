using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements sql server data source.
    /// </summary>
    public class SqlClient<TConfigElement> : DataSource<TConfigElement> where TConfigElement : ConfigElement, new()
    {
        /// <summary>
        /// Gets or sets the connection string where the config table can be found.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the config table name.
        /// </summary>
        public string TableName { get; set; }

        public override string Select(IDictionary<string, string> keys)
        {
            using (var context = new SmartConfigEntities<TConfigElement>(ConnectionString, TableName, keys.Select(k => k.Key)))
            {
                var name = keys[KeyNames.DefaultKeyName];
                var elements = context.ConfigElements.Where(ce => ce.Name == name).ToList() as IEnumerable<TConfigElement>;
                elements = ApplyFilters(elements, keys);

                var element = elements.SingleOrDefault();
                return element == null ? null : element.Value;
            };
        }

        public override void Update(IDictionary<string, string> keys, string value)
        {
            using (var context = new SmartConfigEntities<TConfigElement>(ConnectionString, TableName, keys.Select(k => k.Key)))
            {
                // create default entity
                var newEntity = new TConfigElement()
                {
                    Name = keys[KeyNames.DefaultKeyName],
                    Value = value
                };

                // set values for custom properties
                foreach (var property in newEntity.CustomProperties)
                {
                    newEntity.SetStringDelegates[property.Name](keys[property.Name]);
                }

                // helper Func for creating entity keys
                var createEntityKey = new Func<DbContext, TConfigElement, EntityKey>((c, e) =>
                {
                    const string entitySetName = "ConfigElements";
                    var entityKey = ((IObjectContextAdapter)c).ObjectContext.CreateEntityKey(entitySetName, e);
                    return entityKey;
                });

                // create an entity key for the new entity so that EF can compare it with existing entities
                var newEntityKey = createEntityKey(context, newEntity);

                // try to get an existing entity from the database by entity key
                var name = keys[KeyNames.DefaultKeyName];
                var elements = context.ConfigElements.Where(x => x.Name == name).ToList() as IEnumerable<TConfigElement>;
                elements = ApplyFilters(elements, keys);

                var existingEntity = elements.SingleOrDefault(e => createEntityKey(context, e) == newEntityKey);

                if (existingEntity != null)
                {
                    existingEntity.Value = value;
                }
                else
                {
                    context.ConfigElements.Add(newEntity);
                    context.Entry(newEntity).State = EntityState.Added;
                }

                context.SaveChanges();
            };
        }
    }
}
