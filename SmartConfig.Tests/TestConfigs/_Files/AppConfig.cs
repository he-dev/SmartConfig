using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Converters;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class AppConfig
    {
        public class ConnectionStrings
        {
            public static string TestName;
        }

        public class AppSettings
        {
            public static string TestKey;
        }
    }
}
