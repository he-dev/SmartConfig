using System.Collections.Generic;
using SmartConfig.Converters;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class JsonSettings
    {
        [ObjectConverter(typeof(JsonConverter))]
        public static List<int> ListInt32Setting { get; set; }
    }
}
