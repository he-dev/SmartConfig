using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Tests
{
    // http://stackoverflow.com/a/13992028/235671

    class TestDbInitializer : System.Data.Entity.DropCreateDatabaseAlways<SmartConfigEntities>
    {
        protected override void Seed(SmartConfigEntities context)
        {
            var testConfig = new[]
            {
                "ABC|1.0.0|StringField|abc",
                "ABC|1.2.0|Int32Field|123",
                "ABC|2.1.1|StringField|jkl",
                "JKL|3.2.4|StringField|xyz",
            }
            .ToConfigElements();

            foreach (var configElement in testConfig)
            {
                context.ConfigElements.Add(configElement);
            }
            context.SaveChanges();
        }
    }
}
