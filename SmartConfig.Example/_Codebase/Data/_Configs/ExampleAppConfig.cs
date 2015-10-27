namespace SmartConfig.Example.Data
{
    [SmartConfig]
    static class ExampleAppConfig
    {
        public static class AppSettings
        {
            public static string Environment { get; set; }
            public static string Greeting { get; set; }

            [Optional]
            public static string Farewell { get; set; } = "Good bye!";
        }

        public static class ConnectionStrings
        {
            public static string ExampleDb { get; set; }

            [Optional]
            public static string TestDb { get; set; }
        }        
    }
}
