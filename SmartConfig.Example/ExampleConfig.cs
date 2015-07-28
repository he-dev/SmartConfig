using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Example
{
    [SmartConfig]
    static class ExampleConfig
    {
        public static class AppSettings
        {
            public static string Greeting;

            [Optional]
            public static string Farewell = "Good bye!";
        }
    }
}
