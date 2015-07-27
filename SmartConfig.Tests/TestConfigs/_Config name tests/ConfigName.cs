using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig(Name = "ABCD")]
    public class ConfigName
    {
        [AllowNull]
        public static string StringField;
    }
}
