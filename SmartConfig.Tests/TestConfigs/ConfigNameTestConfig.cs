using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig(Name = "ABCD")]
    public class ConfigNameTestConfig
    {
        [AllowNull]
        public static string StringField;
    }
}
