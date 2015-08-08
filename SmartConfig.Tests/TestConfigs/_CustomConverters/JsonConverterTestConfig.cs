using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Converters;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class JsonField
    {
        [ObjectConverter(typeof(JsonConverter))]
        public static List<int> ListInt32Field;
    }
}
