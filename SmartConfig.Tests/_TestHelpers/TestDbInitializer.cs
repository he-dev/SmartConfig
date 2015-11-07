using System.Data.Entity;
using System.Linq;
using SmartConfig.Data;

namespace SmartConfig.Tests.Data
{
    class TestDbInitializer : DropCreateDatabaseAlways<SmartConfigContext<TestSetting>>
    {
        protected override void Seed(SmartConfigContext<TestSetting> context)
        {
            var testSettings = new[]
            {
                "ABC|1.0.0|StringSetting|abc",
                "ABC|1.2.0|Int32Setting|123",
                "ABC|2.1.1|StringSetting|jkl",
                "JKL|3.2.4|StringSetting|xyz",
            }
            .Select(x => new TestSetting(x));

            foreach (var setting in testSettings)
            {
                context.Settings.Add(setting);
            }
            context.SaveChanges();
            base.Seed(context);
        }
    }
}
