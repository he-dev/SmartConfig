using SmartConfig.Data;

namespace SmartConfig.Examples
{
    [SmartConfig]
    [SettingName("Examples")]
    static class ExampleAppConfig
    {
        public static class AppSettings
        {
            public static string Environment { get; set; }

            public static string Greeting { get; set; } = "Hallo SmartConfig!";

            [Optional]
            public static string Farewell { get; set; } = "Good bye!";
        }

        public static class ConnectionStrings
        {
            public static string ExampleDb { get; set; }

            [Optional]
            public static string MissingExampleDb { get; set; }
        }
    }    
}
