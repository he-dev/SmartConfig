namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class VersionTestConfig
    {
        [Optional]
        public static string StringField { get; set; }
    }
}
