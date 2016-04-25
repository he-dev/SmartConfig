using System.Data.Entity;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SqlServer.Tests
{
    internal class TestDbInitializer : DropCreateDatabaseAlways<SqlServerContext<CustomTestSetting>>
    {
        protected override void Seed(SqlServerContext<CustomTestSetting> dbContext)
        {
            // let's create some test data
            var testSettings = new []
            {
                new CustomTestSetting { Name = "Foo", Value = "Grault", Environment = "*", Version = "*" },
                new CustomTestSetting { Name = "baz.Foo", Value = "Fred", Environment = "*", Version = "*" },
                new CustomTestSetting { Name = "Foo", Value = "Plugh", Environment = "corge", Version = "1.3.0" },

                new CustomTestSetting { Name = "Bar", Value = "Baz", Environment = "corge", Version = "2.4.0" },
            };

            foreach (var setting in testSettings)
            {
                dbContext.Settings.Add(setting);
            }

            dbContext.SaveChanges();
            base.Seed(dbContext);
        }
    }
}
