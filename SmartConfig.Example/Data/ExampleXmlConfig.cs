using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Example.Data
{
    [SmartConfig]
    static class ExampleXmlConfig
    {
        public static class AppSettings
        {
            public static string Environment;
            public static string Greeting;

            [Optional]
            public static string Farewell = "Good bye!";
        }

        public static class ConnectionStrings
        {
            public static string ExampleDb;

            [Optional]
            public static string TestDb;
        }        
    }
}
