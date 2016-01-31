namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class StringSettings
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        public static string StringSetting { get; set; }
    }    
}
