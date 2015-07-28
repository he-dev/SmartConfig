using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            SmartConfigManager.Load(typeof(ExampleConfig), new AppConfig());

            Console.WriteLine(ExampleConfig.AppSettings.Greeting);
            Console.WriteLine(ExampleConfig.AppSettings.Farewell);
        }
    }
}
