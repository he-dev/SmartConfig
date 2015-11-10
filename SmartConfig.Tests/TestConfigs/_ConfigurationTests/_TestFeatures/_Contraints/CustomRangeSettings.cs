namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class CustomRangeSettings
    {
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        [Range(typeof(int), "3", "7")]
        public static int Int32Field { get; set; }
    }
}
