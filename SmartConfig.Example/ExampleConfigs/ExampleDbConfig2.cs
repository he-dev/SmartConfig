using SmartConfig.Data;

namespace SmartConfig.Example.Data
{
    [SmartConfig(Name = "ExampleApp2")]
    static class ExampleDbConfig2
    {
        public static class Properties
        {
            public static IDataSource DataSource => new DbSource<Setting>("name=abc", "xyz");

            public static class CustomKeys
            {
                public static string Environment { get; set; } = "ABC";
                public static string Version { get; set; } = "2.0.0";
            }
        }

        public static string GoodBye;
    }
}
