namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    [SettingName("ABC")]
    public static class CustomConfigName
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        [Optional]
        public static string StringSetting { get; set; }
    }
}
