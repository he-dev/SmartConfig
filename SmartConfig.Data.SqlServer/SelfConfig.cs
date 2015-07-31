using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    [SmartConfig]
    static class SelfConfig
    {
        public static class AppSettings
        {
            [Optional]
            public static string Environment = string.Empty;
        }
    }
}
