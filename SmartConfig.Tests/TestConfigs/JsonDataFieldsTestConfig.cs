using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public class JsonDataFieldsTestConfig
    {
        [JsonObjectConverter]
        public static List<float> ListFloatField;
    }
}
