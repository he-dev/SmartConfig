using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements sql server data source.
    /// </summary>
    public class SqlServer : DataSource
    {
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

            using (var context = new SmartConfigEntities(ConnectionString))
            {
                return context.ConfigElements.Where(ce => ce.Name == name).ToList();
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

            using (var context = new SmartConfigEntities(ConnectionString))
            {
                context.ConfigElements.Add(configElement);
                context.SaveChanges();
            };
        }
    }
}
