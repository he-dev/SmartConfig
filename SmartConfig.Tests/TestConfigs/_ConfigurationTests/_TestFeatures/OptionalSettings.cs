namespace SmartConfig.Core.Tests.TestConfigs
{
    [SmartConfig]
    public static class OptionalSettings
    {
        [Optional]
        public static string StringSetting { get; set; } = "abc";
    }
}
