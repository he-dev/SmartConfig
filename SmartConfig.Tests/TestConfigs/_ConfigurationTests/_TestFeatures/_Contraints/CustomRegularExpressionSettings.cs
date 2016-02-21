namespace SmartConfig.Core.Tests.TestConfigs
{
    [SmartConfig]
    public static class CustomRegularExpressionSettings
    {
        [RegularExpression(@"\d{2}")]
        public static string StringSetting { get; set; }
    }
}
