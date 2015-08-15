using System;
using System.Collections.Generic;
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
            AppConfigExample();
            //BasicSqlClientExample();
            //CustomSqlClientExample();

            Console.ReadKey();
        }

        private static void AppConfigExample()
        {

            SmartConfigManager.Load(typeof(ExampleAppConfig), new XmlConfig<CustomConfigElement>()
            {
                FileName = @"Data\XmlConfig.xml",
                Keys = new Dictionary<string, string>()
                {
                    { KeyNames.EnvironmentKeyName, "ABC" }
                },
                Filters = new Dictionary<string, FilterByFunc<CustomConfigElement>>()
                {
                    { KeyNames.EnvironmentKeyName, Filters.FilterByString }
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
                Keys = new Dictionary<string, string>() { { KeyNames.EnvironmentKeyName, "ABC" } },
                Filters = new Dictionary<string, FilterByFunc<CustomConfigElement>>()
                {
                    { KeyNames.EnvironmentKeyName, Filters.FilterByString },
                    { KeyNames.VersionKeyName, Filters.FilterByVersion }
                }
            });

            Console.WriteLine(ExampleDbConfig2.GoodBye);
        }       
    }
}
