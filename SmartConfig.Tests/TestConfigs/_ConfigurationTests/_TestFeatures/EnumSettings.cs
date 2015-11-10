namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class EnumSettings
    {
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        public static TestEnum EnumSetting { get; set; }
    }
}
