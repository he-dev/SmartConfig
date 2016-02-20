namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    [SettingName("ABC")]
    public static class CustomConfigName
    {
        [Optional]
        public static string StringSetting { get; set; }
    }
}
