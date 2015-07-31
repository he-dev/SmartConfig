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
            SmartConfigManager.Load(typeof(ExampleAppConfig), new AppConfig());

            Console.WriteLine(ExampleAppConfig.AppSettings.Greeting);
            Console.WriteLine(ExampleAppConfig.AppSettings.Farewell);

            SmartConfigManager.Update(() => ExampleAppConfig.AppSettings.Farewell, "Bye!");
            Console.WriteLine(ExampleAppConfig.AppSettings.Farewell);

            // --

            SmartConfigManager.Load(typeof(ExampleDbConfig1), new SqlClient<BasicConfigElement>()
            {
                ConnectionString = ExampleAppConfig.ConnectionStrings.ExampleDb,
                TableName = "ExampleConfigTable",
            });

            Console.WriteLine(ExampleDbConfig1.Welcome);

            // --

            SmartConfigManager.Load(typeof(ExampleDbConfig2), new SqlClient<CustomConfigElement>()
            {
                ConnectionString = ExampleAppConfig.ConnectionStrings.ExampleDb,
                TableName = "ExampleConfigTable",
                Keys = new Dictionary<string, string>() { { "Environment", "ABC" } },
                FilterBy = FilterBy
            });

            Console.WriteLine(ExampleDbConfig2.GoodBye);

            Console.ReadKey();
        }

        private static IEnumerable<CustomConfigElement> FilterBy(IEnumerable<CustomConfigElement> elements, KeyValuePair<string, string> keyValue)
        {
            switch (keyValue.Key)
            {
            case "Environment": return Filters.FilterByEnvironment(elements, keyValue.Value).Cast<CustomConfigElement>();
            case "Version": return Filters.FilterBySemanticVersion(elements, keyValue.Value).Cast<CustomConfigElement>();
            default: throw new IndexOutOfRangeException("Filter function not found.");
            }
        }
    }
}
