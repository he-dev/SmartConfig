using System;
using System.ComponentModel.DataAnnotations;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class CustomRangeSettings
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        [Range(Type = typeof(int), Min = "3", Max = "7")]
        public static int Int32Field { get; set; }
    }
}
