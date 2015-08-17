using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;
using SmartConfig.Example.Data;

namespace SmartConfig.Example
{
    static class Program
    {
        static void Main(string[] args)
        {
            Logger.Log = m => Debug.WriteLine(m);

            //AppConfigExample();
            //BasicSqlClientExample();
            //CustomSqlClientExample();
            XmlConfigExample();

            Console.ReadKey();
        }

        private static void AppConfigExample()
        {
            // SmartConfigBuilder.Load(typeof(ExampleAppConfig)).From<XmlConfig>(ds => ds.FileName = @"abc.xml")
            SmartConfigManager.Load(typeof(ExampleAppConfig), new XmlConfig<CustomConfigElement>()
            {
                FileName = @"Data\XmlConfig.xml",
                CustomKeys = new[]
                {
                    new CustomKey<CustomConfigElement>()
                    {
                        Name = KeyNames.EnvironmentKeyName,
                        Value = "ABC",
                        Filter = Filters.FilterByString
                    },
                }
            });

            return;

            SmartConfigManager.Load(typeof(ExampleAppConfig), new AppConfig());

            Console.WriteLine(ExampleAppConfig.AppSettings.Greeting);
            Console.WriteLine(ExampleAppConfig.AppSettings.Farewell);

            SmartConfigManager.Update(() => ExampleAppConfig.AppSettings.Farewell, "Bye!");
            Console.WriteLine(ExampleAppConfig.AppSettings.Farewell);

            SmartConfigManager.Update(() => ExampleAppConfig.ConnectionStrings.TestDb, "Test connection string");
        }

        private static void BasicSqlClientExample()
        {
            SmartConfigManager.Load(typeof(ExampleDbConfig1), new SqlClient<ConfigElement>()
            {
                ConnectionString = ExampleAppConfig.ConnectionStrings.ExampleDb,
                TableName = "ExampleConfigTable",
            });

            Console.WriteLine(ExampleDbConfig1.Welcome);
        }

        private static void CustomSqlClientExample()
        {
            SmartConfigManager.Load(typeof(ExampleDbConfig2), new SqlClient<CustomConfigElement>()
            {
                ConnectionString = ExampleAppConfig.ConnectionStrings.ExampleDb,
                TableName = "ExampleConfigTable",                
            }
            .AddCustomKey(k => k.HasName(KeyNames.EnvironmentKeyName).HasValue("ABC").HasFilter(Filters.FilterByString))
            .AddCustomKey(k => k.HasName(KeyNames.VersionKeyName).HasValue("2.0.0").HasFilter(Filters.FilterByVersion)));

            Console.WriteLine(ExampleDbConfig2.GoodBye);
        }

        private static void XmlConfigExample()
        {
            SmartConfigManager.Load(typeof(ExampleXmlConfig), new XmlConfig<CustomConfigElement>()
            {
                FileName = @"Data\XmlConfig.xml",
            }
            .AddCustomKey(k => k.HasName(KeyNames.EnvironmentKeyName).HasValue("ABC").HasFilter(Filters.FilterByString)));
        }
    }
}
