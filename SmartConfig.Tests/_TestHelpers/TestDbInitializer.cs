using System.Data.Entity;
using System.Linq;
using SmartConfig.Data;

namespace SmartConfig.Tests
{
    class TestDbInitializer : DropCreateDatabaseAlways<SmartConfigDbContext<TestSetting>>
    {
        protected override void Seed(SmartConfigDbContext<TestSetting> dbContext)
        {
            // let's create some test data
            var testSettings = new []
            {
                new TestSetting { Name = "StringSetting", Value = "foo", Environment = "A", Version = "1.0.0" },
                new TestSetting { Name = "StringSetting", Value = "baz", Environment = "A", Version = "1.2.1" },
                new TestSetting { Name = "Int32Setting", Value = "123", Environment = "A", Version = "2.1.1" },

                new TestSetting { Name = "StringSetting", Value = "bar", Environment = "B", Version = "3.2.4" },

                new TestSetting { Name = "app2.StringSetting", Value = "qux", Environment = "A", Version = "5.0.4" },
                new TestSetting { Name = "app2.Int32Setting", Value = "890", Environment = "A", Version = "5.0.4" },
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
