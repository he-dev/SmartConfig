using System;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class DateTimeFormatTestConfig
    {
        [DateTimeFormat("ddMMyy")]
        public static DateTime DateTimeField { get; set; }
    }
}
