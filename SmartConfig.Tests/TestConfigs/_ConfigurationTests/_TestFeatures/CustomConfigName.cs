namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig(Name = "ABC")]
    public static class CustomConfigName
    {
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        [Optional]
        public static string StringSetting { get; set; }
    }
}
