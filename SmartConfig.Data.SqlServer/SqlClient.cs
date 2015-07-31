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
    public class SqlClient<TConfigElement> : DataSourceBase where TConfigElement : BasicConfigElement
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

        public FilterByCallback<TConfigElement> FilterBy { get; set; }

        public override string Select(IDictionary<string, string> keys)
        {
            using (var context = new SmartConfigEntities<TConfigElement>(ConnectionString, TableName))
            {
                var allKeys = Utilities.CombineDictionariesWithoutName(keys, Keys);

                var name = keys["Name"];
                var elements = context.ConfigElements.Where(ce => ce.Name == name).ToList() as IEnumerable<TConfigElement>;
                if (FilterBy != null)
                {
                    elements = allKeys.Aggregate(elements, (current, keyValue) => FilterBy(current, keyValue));
                }

                var element = elements.SingleOrDefault();
                return element == null ? null : element.Value;
            };
        }

        public override void Update(IDictionary<string, string> keys, string value)
        {
            //using (var context = new SmartConfigEntities<TConfigElement>(ConnectionString, TableName))
            //{
            //    var name = compositeKey["Name"];
            //    var element = context.ConfigElements.FirstOrDefault(ce => ce.Name == name);
            //    if (element == null)
            //    {
            //        element = new BasicConfigElement()
            //        {
            //            Name = name,
            //            Value = value
            //        };
            //    }
            //    else
            //    {
            //        element.Value = element.Value;
            //    }
            //    context.SaveChanges();
            //};
        }
    }
}
