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

            SmartConfigManager.Load(typeof(ExampleDbConfig), new SqlServer()
            {
                ConnectionString = ExampleAppConfig.ConnectionStrings.ExampleDb,
                TableName = "ExampleConfigTable"
            });

            Console.WriteLine(ExampleDbConfig.Colors.FontColor);
        }
    }
}
