using System.Collections.Generic;
using SmartConfig.Converters;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class JsonField
    {
        [ObjectConverter(typeof(JsonConverter))]
        public static List<int> ListInt32Field { get; set; }
    }
}
