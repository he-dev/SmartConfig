namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class NestedSettings
    {
        public static class Properties
        {
            public static SimpleTestDataSource DataSource { get; set; } = new SimpleTestDataSource();
        }

        public static class SubConfig
        {
            public static string SubSetting { get; set; }

            public static class SubSubConfig
            {
                public static string SubSubSetting { get; set; }
            }
        }
    }
}
