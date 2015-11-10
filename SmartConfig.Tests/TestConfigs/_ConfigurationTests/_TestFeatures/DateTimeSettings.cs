using System;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class DateTimeSettings
    {
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        public static DateTime DateTimeSetting { get; set; }
    }
}
