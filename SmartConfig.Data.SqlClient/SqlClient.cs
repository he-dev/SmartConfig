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
        public SqlClient()
        {
            using (var context = new SmartConfigEntities<TConfigElement>(ConnectionString, TableName))
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var objectSet = objectContext.CreateObjectSet<TConfigElement>();
                OrderedKeyNames = objectSet.EntitySet.ElementType.KeyMembers.Select(k => k.Name).ToList();
            }
        }

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
            using (var context = new SmartConfigEntities<TConfigElement>(ConnectionString, TableName))
            {
                var name = keys[SmartConfig.KeyNames.DefaultKeyName];
                var elements = context.ConfigElements.Where(ce => ce.Name == name).ToList() as IEnumerable<TConfigElement>;
                elements = ApplyFilters(elements, keys);

                var element = elements.SingleOrDefault();
                return element == null ? null : element.Value;
            };
        }

        public override void Update(IDictionary<string, string> keys, string value)
        {
            using (var context = new SmartConfigEntities<TConfigElement>(ConnectionString, TableName))
            {
                var entity = context.ConfigElements.Find(OrderedKeyNames.Select(k => keys[k]).Cast<object>().ToArray());

                // there is no such entity yet so create a new one
                if (entity == null)
                {
                    // create a new entity
                    entity = new TConfigElement()
                    {
                        Name = keys[SmartConfig.KeyNames.DefaultKeyName],
                        Value = value
                    };

                    // set keys
                    foreach (var keyName in OrderedKeyNames.Where(k => k != SmartConfig.KeyNames.DefaultKeyName))
                    {
                        entity.SetStringDelegates[keyName](keys[keyName]);
                    }

                    context.ConfigElements.Add(entity);
                }
                // there is already such entity so just update the value
                else
                {
                    entity.Value = value;
                }

                context.SaveChanges();
            };
        }
    }
}
