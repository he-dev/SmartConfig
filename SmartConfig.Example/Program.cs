using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;
using SmartConfig.Example.Data;

namespace SmartConfig.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            AppConfigExample();
            BasicSqlClientExample();
            CustomSqlClientExample();

            Console.ReadKey();
        }

        private static void AppConfigExample()
        {
            SmartConfigManager.Load(typeof(ExampleAppConfig), new AppConfig());

            Console.WriteLine(ExampleAppConfig.AppSettings.Greeting);
            Console.WriteLine(ExampleAppConfig.AppSettings.Farewell);

            SmartConfigManager.Update(() => ExampleAppConfig.AppSettings.Farewell, "Bye!");
            Console.WriteLine(ExampleAppConfig.AppSettings.Farewell);
        }

        private static void BasicSqlClientExample()
        {
            SmartConfigManager.Load(typeof(ExampleDbConfig1), new SqlClient<BasicConfigElement>()
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
                Keys = new Dictionary<string, string>() { { CommonFieldKeys.Environment, "ABC" } },
                FilterBy = FilterBy
            });

            Console.WriteLine(ExampleDbConfig2.GoodBye);
        }

        private static IEnumerable<CustomConfigElement> FilterBy(IEnumerable<CustomConfigElement> elements, KeyValuePair<string, string> keyValue)
        {
            switch (keyValue.Key)
            {
            case CommonFieldKeys.Environment: return CommonFilters.FilterByEnvironment(elements, keyValue.Value).Cast<CustomConfigElement>();
            case CommonFieldKeys.Version: return CommonFilters.FilterBySemanticVersion(elements, keyValue.Value).Cast<CustomConfigElement>();
            default: throw new IndexOutOfRangeException("Filter function not found.");
            }
        }
    }
}
