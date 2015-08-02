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
    public class SqlClient<TConfigElement> : IDataSource where TConfigElement : BasicConfigElement, new()
    {
        public SqlClient()
        {
            Keys = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the connection string where the config table can be found.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the config table name.
        /// </summary>
        public string TableName { get; set; }

        public IDictionary<string, string> Keys { get; set; }

        public FilterByFunc<TConfigElement> FilterBy { get; set; }

        public string Select(IDictionary<string, string> keys)
        {
            using (var context = new SmartConfigEntities<TConfigElement>(ConnectionString, TableName))
            {
                var allKeys = Utilities.CombineDictionaries(keys, Keys);
                var name = keys[CommonKeys.Name];
                var elements = context.ConfigElements.Where(ce => ce.Name == name).ToList() as IEnumerable<TConfigElement>;
                if (FilterBy != null)
                {
                    elements = allKeys.Where(x => x.Key != CommonKeys.Name).Aggregate(elements, (current, keyValue) => FilterBy(current, keyValue));
                }

                var element = elements.SingleOrDefault();
                return element == null ? null : element.Value;
            };
        }

        public void Update(IDictionary<string, string> keys, string value)
        {
            var allKeys = Utilities.CombineDictionaries(keys, Keys);

            using (var context = new SmartConfigEntities<TConfigElement>(ConnectionString, TableName))
            {
                var newEntity = new TConfigElement()
                {
                    Name = keys[CommonKeys.Name],
                    Value = value
                };

                var declaredProperties = typeof(TConfigElement).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (var property in declaredProperties)
                {
#if NET40
                    property.SetValue(newEntity, allKeys[property.Name], null);
#else
                    property.SetValue(newEntity, allKeys[property.Name]);
#endif
                }

                var newEntityKey = ((IObjectContextAdapter)context).ObjectContext.CreateEntityKey("ConfigElements", newEntity);

                // Try to get the entity from the database.
                var name = keys[CommonKeys.Name];
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
