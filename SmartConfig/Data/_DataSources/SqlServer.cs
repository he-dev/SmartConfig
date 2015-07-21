using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements sql server data source.
    /// </summary>
    public class SqlServer : DataSourceBase
    {
        /// <summary>
        /// Gets or sets the connection string where the config table can be found.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the config table name.
        /// </summary>
        public string TableName { get; set; }

        static SqlServer()
        {
            Database.SetInitializer<SmartConfigEntities>(null);
        }

        public override IEnumerable<ConfigElement> Select(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new InvalidOperationException("ConnectionString must not be empty.");
            }

            // >

            using (var context = new SmartConfigEntities(ConnectionString, TableName))
            {
                var result =
                    context
                    .ConfigElements
                    .Where(ce => ce.Name == name)
                    .ToList();
                return result;
            };
        }

        public override void Update(ConfigElement configElement)
        {
            if (configElement == null)
            {
                throw new ArgumentNullException("configElement");
            }

            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new InvalidOperationException("ConnectionString must not be empty.");
            }

            // >

            using (var context = new SmartConfigEntities(ConnectionString, TableName))
            {
                var _configElement =
                    context
                    .ConfigElements
                    .Where(ce =>
                        ce.Environment == configElement.Environment
                        && ce.Version == configElement.Version
                        && ce.Name == configElement.Name)
                    .FirstOrDefault();

                _configElement.Value = configElement.Value;
                context.SaveChanges();
            };
        }
    }
}
