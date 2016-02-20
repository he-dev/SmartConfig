using System;
using System.ComponentModel.DataAnnotations;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class CustomRangeSettings
    {
        [Range(typeof(int), "3", "7")]
        public static int Int32Field { get; set; }
    }
}
