using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Example.Data
{
    [SmartConfig(Name = "ExampleApp", Version = "1.0.0")]
    static class ExampleDbConfig
    {
        [Optional]
        public static string Welcome;

        public static class Colors
        {
            public static Color FontColor;
        }
    }
}
