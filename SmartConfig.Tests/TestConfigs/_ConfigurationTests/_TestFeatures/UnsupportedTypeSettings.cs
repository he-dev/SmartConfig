using System;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class UnsupportedTypeSettings
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        public static Uri UriSetting { get; set; }
    }
}
