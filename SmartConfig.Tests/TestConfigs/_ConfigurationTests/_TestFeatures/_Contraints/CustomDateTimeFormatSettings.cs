using System;

namespace SmartConfig.Core.Tests.TestConfigs
{
    [SmartConfig]
    public static class CustomDateTimeFormatSettings
    {
        [DateTimeFormat("ddMMMyy")]
        public static DateTime DateTimeSetting { get; set; }
    }
}
