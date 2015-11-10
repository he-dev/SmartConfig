using System;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class CustomDateTimeFormatSettings
    {
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        [DateTimeFormat("ddMMMyy")]
        public static DateTime DateTimeField { get; set; }
    }
}
