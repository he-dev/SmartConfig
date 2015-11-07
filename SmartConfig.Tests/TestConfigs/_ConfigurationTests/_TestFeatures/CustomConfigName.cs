namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig(Name = "ABC")]
    public static class CustomConfigName
    {
        [Optional]
        public static string StringSetting { get; set; }
    }
}
