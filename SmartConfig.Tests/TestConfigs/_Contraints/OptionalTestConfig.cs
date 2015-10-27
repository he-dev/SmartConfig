namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class OptionalTestConfig
    {
        [Optional]
        public static string StringField { get; set; } = "xyz";
    }
}
