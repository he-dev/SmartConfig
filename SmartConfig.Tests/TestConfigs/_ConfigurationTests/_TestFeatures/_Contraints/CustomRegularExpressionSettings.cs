namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class CustomRegularExpressionSettings
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        [RegularExpression(@"\d{2}")]
        public static string StringSetting { get; set; }
    }
}
