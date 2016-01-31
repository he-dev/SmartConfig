namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class NonOptionalSettings
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        public static int Int32Setting { get; set; }
    }
}
