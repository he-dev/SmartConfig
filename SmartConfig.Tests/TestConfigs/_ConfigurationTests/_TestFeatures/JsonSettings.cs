using System.Collections.Generic;
using SmartConfig.Converters;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class JsonSettings
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        [ObjectConverter(typeof(JsonConverter))]
        public static List<int> ListInt32Setting { get; set; }
    }
}
