using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class OptionalTestConfig
    {
        [Optional]
        public static string StringField = "xyz";
    }
}
