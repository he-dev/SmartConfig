using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests
{
    // http://stackoverflow.com/a/13992028/235671

    class TestDbInitializer : System.Data.Entity.DropCreateDatabaseAlways<global::SmartConfig.Data.SmartConfigEntities>
    {
        protected override void Seed(global::SmartConfig.Data.SmartConfigEntities context)
        {
            // Add entities to database.

            context.ConfigElements.Add(new global::SmartConfig.Data.ConfigElement()
            {
                Environment = "ABC",
                Version = "1.0.0",
                Name = "ABCD.Int32Field",
                Value = "2"
            });
            context.SaveChanges();
        }
    }
}
