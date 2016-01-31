namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class BooleanSettings
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        public static bool falseSetting { get; set; }
        public static bool trueSetting { get; set; }
    }
}
