using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig(Version = "v2.1.1")]
    public class VersionTestConfig
    {
        public static string StringField;
    }
}
