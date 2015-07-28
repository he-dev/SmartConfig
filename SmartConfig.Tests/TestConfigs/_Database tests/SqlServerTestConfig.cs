using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig(Version = "1.2.0")]
    public static class SqlServerTestConfig
    {
        public static int Int32Field;
    }
}
