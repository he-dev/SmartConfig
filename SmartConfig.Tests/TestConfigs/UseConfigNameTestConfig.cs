using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig(UseConfigName = true)]
    public class UseConfigNameTestConfig
    {
        public static string StringField;
    }
}
