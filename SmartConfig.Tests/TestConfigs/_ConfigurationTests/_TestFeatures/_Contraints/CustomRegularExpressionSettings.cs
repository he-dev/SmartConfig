namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class CustomRegularExpressionSettings
    {
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        [RegularExpression(Pattern = @"\d{2}")]
        public static string StringSetting { get; set; }
    }
}
