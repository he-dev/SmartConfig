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

            AppConfigExample();
            //BasicSqlClientExample();
            //CustomSqlClientExample();

            Console.ReadKey();
        }

        private static void AppConfigExample()
        {
            // SmartConfigBuilder.Load(typeof(ExampleAppConfig)).From<XmlConfig>(ds => ds.FileName = @"abc.xml")
            SmartConfigManager.Load(typeof(ExampleAppConfig), new XmlConfig<CustomConfigElement>()
            {
                FileName = @"Data\XmlConfig.xml",                
                Keys = new []
                {
                    new KeyInfo<CustomConfigElement>()
                    {
                        KeyName = KeyNames.EnvironmentKeyName,
                        KeyValue = "ABC",
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
                Keys = new[]
                {
                    new KeyInfo<CustomConfigElement>()
                    {
                        KeyName = KeyNames.EnvironmentKeyName,
                        KeyValue = "ABC",
                        Filter = Filters.FilterByString
                    },
                    new KeyInfo<CustomConfigElement>()
                    {
                        KeyName = KeyNames.VersionKeyName,
                        Filter = Filters.FilterByVersion
                    }
                }                
            });

            Console.WriteLine(ExampleDbConfig2.GoodBye);
        }
    }
}
