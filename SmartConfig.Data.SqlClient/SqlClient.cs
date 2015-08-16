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
                elements = keys
                    .Where(x => x.Key != KeyNames.DefaultKeyName)
                    .Aggregate(elements, (current, item) => _keys[item.Key].Filter(current, item));

                var element = elements.SingleOrDefault();
                return element == null ? null : element.Value;
            };
        }

        public override void Update(IDictionary<string, string> keys, string value)
        {
            using (var context = new SmartConfigEntities<TConfigElement>(ConnectionString, TableName, keys.Select(k => k.Key)))
            {
                var newEntity = new TConfigElement()
                {
                    Name = keys[KeyNames.DefaultKeyName],
                    Value = value
                };

                var declaredProperties = typeof(TConfigElement).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (var property in declaredProperties)
                {
#if NET40
                    property.SetValue(newEntity, keys[property.Name], null);
#else
                    property.SetValue(newEntity, keys[property.Name]);
#endif
                }

                var newEntityKey = ((IObjectContextAdapter)context).ObjectContext.CreateEntityKey("ConfigElements", newEntity);

                // Try to get the entity from the database.
                var name = keys[KeyNames.DefaultKeyName];
                var elements = context.ConfigElements.Where(x => x.Name == name).ToList() as IEnumerable<TConfigElement>;
                var existingEntity = elements.SingleOrDefault(e => ((IObjectContextAdapter)context).ObjectContext.CreateEntityKey("ConfigElements", e) == newEntityKey);

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
