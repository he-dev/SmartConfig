using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public class UpdateJsonTestConfig
    {
        [JsonObjectConverter]
        [AllowNull]
        public static List<float> ListFloatField;
    }
}
