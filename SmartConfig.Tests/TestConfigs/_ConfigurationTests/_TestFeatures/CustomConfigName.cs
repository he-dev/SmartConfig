namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class CustomConfigName
    {
        public static class Properties
        {
            public static string Name => "ABC";
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        [Optional]
        public static string StringSetting { get; set; }
    }
}
