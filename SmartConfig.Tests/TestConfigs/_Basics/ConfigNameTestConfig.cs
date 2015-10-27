namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig(Name = "ABCD")]
    public static class ConfigNameTestConfig
    {
        [Optional]
        public static string StringField { get; set; }
    }
}
