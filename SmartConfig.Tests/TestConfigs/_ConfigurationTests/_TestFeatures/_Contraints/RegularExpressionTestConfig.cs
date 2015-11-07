namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class RegularExpressionTestConfig
    {
        [RegularExpression(@"\d{2}")]
        public static string StringField { get; set; }
    }
}
