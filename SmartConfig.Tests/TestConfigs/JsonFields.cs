using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Converters;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public class JsonFields
    {
        [ObjectConverter(typeof(JsonConverter))]
        public static List<Int32> ListInt32Field;
    }
}
