using SmartConfig.Data;

namespace SmartConfig.Example.ExampleConfigs
{
    [SmartConfig]
    [SettingName("MyTestConfig")]
    static class ExampleAppConfig
    {
        public static class AppSettings
        {
            public static string Environment { get; set; }
            public static string Greeting { get; set; }

            [Optional]
            public static string Farewell { get; set; } = "Good bye!";

            //public static class EvenDeeper
            //{
            //    public static string Deep { get; set; }
            //}
        }

        public static class ConnectionStrings
        {
            public static string ExampleDb { get; set; }

            [Optional]
            public static string TestDb { get; set; }
        }
    }

    [SmartConfig]
    static class TestConfig
    {
        public static class Properties
        {
            public static string Name => "abc";

            public static IDataSource DataSource => new DbSource<Setting>("name=abc", "xyz");

            public static class CustomKeys
            {
                public static string Environment { get; set; } = "ABC";
                public static string Version { get; set; } = "1.0.0";
            }
        }
    }
}
