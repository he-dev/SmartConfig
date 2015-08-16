using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig(Version = "Invalid Version!")]
    public static class InvalidFieldKeyTestConfig
    {
        public static string StringField;
    }
}
