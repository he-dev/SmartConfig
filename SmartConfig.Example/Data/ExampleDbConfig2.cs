using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Example.Data
{
    [SmartConfig(Name = "ExampleApp2")]
    [CustomKey("Version=2.0.0")]
    static class ExampleDbConfig2
    {
        public static string GoodBye;
    }
}
