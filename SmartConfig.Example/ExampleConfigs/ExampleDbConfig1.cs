using SmartConfig.Data;

namespace SmartConfig.Example.ExampleConfigs
{
    [SmartConfig(Name = "ExampleApp1")]
    static class ExampleDbConfig1
    {
        public static class Properties
        {
            public static IDataSource DataSource => new DbSource<Setting>(ExampleAppConfig.ConnectionStrings.ExampleDb, "ExampleConfigTable");

            public static class CustomKeys
            {
                public static string Environment { get; set; } = "ABC";
                public static string Version { get; set; } = "1.0.0";
            }
        }

        [Optional]
        public static string Welcome { get; set; } = "Hello World!";
    }
}
