using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Converters;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public class AppConfigTestConfig
    {
        public class ConnectionStrings
        {
            public static string SmartConfigEntities;
        }

        public class AppSettings
        {
            public static string Environment;
        }
    }
}
