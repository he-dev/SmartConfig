using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig(Version = "2.1.1")]
    public static class FieldKeyTestConfig
    {
        [Optional]
        public static string StringField;
    }
}
